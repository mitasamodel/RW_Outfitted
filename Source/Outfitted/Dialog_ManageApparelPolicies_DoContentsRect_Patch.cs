using HarmonyLib;
using RimWorld;
using RW_Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Outfitted
{
	[HarmonyPatch(typeof(Dialog_ManageApparelPolicies), "DoContentsRect")]
	internal static class Dialog_ManageApparelPolicies_DoContentsRect_Patch
	{
		private const float marginVertical = 10f;
		private const float marginLeft = 320f;
		private const float marginRight = 10f;
		private const float marginTop = 10f;
		private const float marginBottom = 55f;
		private const float MaxValue = 2.5f;
		private static readonly FloatRange MinMaxTemperatureRange = new FloatRange(-100f, 100f);
		private static Vector2 scrollPosition = Vector2.zero;
		private static bool guiChanged = false;
		private static string statFilterBuffer = "";

		private static void Prefix(ref Rect rect) => rect.width *= 0.5f;

		private static void Postfix(Rect rect, Dialog_ManageApparelPolicies __instance)
		{
			ExtendedOutfit extendedOutfit = (ApparelPolicy)AccessTools.Property(typeof(Dialog_ManagePolicies<ApparelPolicy>), "SelectedPolicy").GetValue((object)__instance, (object[])null) as ExtendedOutfit;
			Dialog_ManageApparelPolicies_DoContentsRect_Patch.DrawCloneButton(extendedOutfit);
			if (extendedOutfit == null)
				return;
			float num1 = 0.0f;
			if (Outfitted.isSaveStorageSettingsEnabled)
			{
				float num2 = num1 + 40f;
			}
			Rect rect1 = new Rect(rect.xMax + 10f, 110f, 300f, rect.height - 60f);
			Dialog_ManageApparelPolicies_DoContentsRect_Patch.guiChanged = false;
			GUI.BeginGroup(rect1);
			Vector2 zero = Vector2.zero;
			Dialog_ManageApparelPolicies_DoContentsRect_Patch.DrawDeadmanToogle(extendedOutfit, ref zero, rect1);
			Dialog_ManageApparelPolicies_DoContentsRect_Patch.DrawAutoWorkPrioritiesToggle(extendedOutfit, ref zero, rect1);
			Dialog_ManageApparelPolicies_DoContentsRect_Patch.DrawAutoTempToggle(extendedOutfit, ref zero, rect1);
			if (!extendedOutfit.AutoTemp)
				Dialog_ManageApparelPolicies_DoContentsRect_Patch.DrawTemperatureStats(extendedOutfit, ref zero, rect1);
			else
				Dialog_ManageApparelPolicies_DoContentsRect_Patch.DrawAutoTempOffsetInput(extendedOutfit, ref zero, rect1);
			zero.y += 10f;
			Dialog_ManageApparelPolicies_DoContentsRect_Patch.DrawApparelStats(extendedOutfit, zero, rect1);
			if (Dialog_ManageApparelPolicies_DoContentsRect_Patch.guiChanged && extendedOutfit != null)
				Outfitted.Notify_OutfitChanged(extendedOutfit.id);
			GUI.EndGroup();
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;
		}

		private static void DrawCloneButton(ExtendedOutfit selectedOutfit)
		{
			if (!Widgets.ButtonText(new Rect((float)(700.0 * 0.75), 0.0f, 150f, 28f), (string)"CommandCopyZoneSettingsLabel".Translate()))
				return;
			if (selectedOutfit == null)
			{
				Messages.Message((string)"NoOutfitSelected".Translate(), MessageTypeDefOf.RejectInput, false);
			}
			else
			{
				List<FloatMenuOption> options = new List<FloatMenuOption>();
				foreach (ApparelPolicy allOutfit in Current.Game.outfitDatabase.AllOutfits)
				{
					ApparelPolicy outfit = allOutfit;
					if (outfit != selectedOutfit)
						options.Add(new FloatMenuOption(outfit.label, (Action)(() => selectedOutfit.CopyFrom((ExtendedOutfit)outfit))));
				}
				Find.WindowStack.Add(new FloatMenu(options));
			}
		}

		private static void DrawDeadmanToogle(
		  ExtendedOutfit selectedOutfit,
		  ref Vector2 cur,
		  Rect canvas)
		{
			Rect rect = new Rect(cur.x, cur.y, canvas.width, 24f);
			int num1 = selectedOutfit.PenaltyWornByCorpse ? 1 : 0;
			Widgets.CheckboxLabeled(rect, ResourceBank.Strings.PenaltyWornByCorpse, ref selectedOutfit.PenaltyWornByCorpse);
			int num2 = selectedOutfit.PenaltyWornByCorpse ? 1 : 0;
			if (num1 != num2)
				guiChanged = true;
			TooltipHandler.TipRegion(rect, new TipSignal(ResourceBank.Strings.PenaltyWornByCorpseTooltip));
			cur.y += rect.height;
		}

		private static void DrawAutoWorkPrioritiesToggle(
		  ExtendedOutfit outfit,
		  ref Vector2 pos,
		  Rect canvas)
		{
			Rect rect = new Rect(pos.x, pos.y, canvas.width, 24f);
			int num1 = outfit.AutoWorkPriorities ? 1 : 0;
			Widgets.CheckboxLabeled(rect, ResourceBank.Strings.AutoWorkPriorities, ref outfit.AutoWorkPriorities);
			int num2 = outfit.AutoWorkPriorities ? 1 : 0;
			if (num1 != num2)
				guiChanged = true;
			TooltipHandler.TipRegion(rect, new TipSignal(ResourceBank.Strings.AutoWorkPrioritiesTooltip));
			pos.y += rect.height;
		}

		private static void DrawAutoTempToggle(ExtendedOutfit outfit, ref Vector2 pos, Rect canvas)
		{
			Rect rect = new Rect(pos.x, pos.y, canvas.width, 24f);
			bool autoTemp = outfit.AutoTemp;
			Widgets.CheckboxLabeled(rect, ResourceBank.Strings.AutoTemp, ref autoTemp);
			if (autoTemp != outfit.AutoTemp)
			{
				outfit.AutoTemp = autoTemp;
				guiChanged = true;
			}
			TooltipHandler.TipRegion(rect, new TipSignal(ResourceBank.Strings.AutoTempTooltip));
			pos.y += rect.height;
		}

		private static void DrawAutoTempOffsetInput(
		  ExtendedOutfit outfit,
		  ref Vector2 pos,
		  Rect canvas)
		{
			Rect rect = new Rect(pos.x, pos.y, canvas.width, 24f);
			int autoTempOffset1 = outfit.autoTempOffset;
			string editBuffer = outfit.autoTempOffset.ToString();
			Widgets.IntEntry(rect, ref outfit.autoTempOffset, ref editBuffer);
			int autoTempOffset2 = outfit.autoTempOffset;
			if (autoTempOffset1 != autoTempOffset2)
				guiChanged = true;
			TooltipHandler.TipRegion(rect, new TipSignal(ResourceBank.Strings.AutoTempOffsetTooltip));
			pos.y += rect.height;
		}

		private static void DrawTemperatureStats(
		  ExtendedOutfit selectedOutfit,
		  ref Vector2 cur,
		  Rect canvas)
		{
			Rect rect1 = new Rect(cur.x, cur.y, canvas.width, 30f);
			cur.y += 30f;
			Text.Anchor = TextAnchor.LowerLeft;
			string preferedTemperature = ResourceBank.Strings.PreferedTemperature;
			Widgets.Label(rect1, preferedTemperature);
			Text.Anchor = TextAnchor.UpperLeft;
			GUI.color = Color.grey;
			Widgets.DrawLineHorizontal(cur.x, cur.y, canvas.width);
			GUI.color = Color.white;
			cur.y += 10f;
			Rect canvas1 = new Rect(cur.x, cur.y, canvas.width - 20f, 40f);
			Rect rect2 = new Rect(canvas1.xMax + 4f, cur.y + 10f, 16f, 16f);
			cur.y += 40f;
			FloatRange range;
			if (selectedOutfit.targetTemperaturesOverride)
			{
				range = selectedOutfit.targetTemperatures;
				GUI.color = Color.white;
			}
			else
			{
				range = MinMaxTemperatureRange;
				GUI.color = Color.grey;
			}
			FloatRange temperatureRange = MinMaxTemperatureRange;
			FloatRange floatRange = range;
			Widgets_FloatRange.FloatRange(canvas1, 123123123, ref range, temperatureRange, ToStringStyle.Temperature);
			if ((double)Math.Abs(floatRange.min - range.min) > 0.0001 || (double)Math.Abs(floatRange.max - range.max) > 0.0001)
				guiChanged = true;
			GUI.color = Color.white;
			if ((double)Math.Abs(range.min - selectedOutfit.targetTemperatures.min) > 0.0001 || (double)Math.Abs(range.max - selectedOutfit.targetTemperatures.max) > 0.0001)
			{
				selectedOutfit.targetTemperatures = range;
				selectedOutfit.targetTemperaturesOverride = true;
				guiChanged = true;
			}
			if (!selectedOutfit.targetTemperaturesOverride)
				return;
			if (Widgets.ButtonImage(rect2, ResourceBank.Textures.ResetButton))
			{
				selectedOutfit.targetTemperaturesOverride = false;
				selectedOutfit.targetTemperatures = MinMaxTemperatureRange;
				guiChanged = true;
			}
			TooltipHandler.TipRegion(rect2, new TipSignal(ResourceBank.Strings.TemperatureRangeReset));
		}

		private static void DrawApparelStats(ExtendedOutfit selectedOutfit, Vector2 cur, Rect canvas)
		{
			Rect rect1 = new Rect(cur.x, cur.y, canvas.width, 22f);
			cur.y += 22f;
			Text.Anchor = TextAnchor.LowerLeft;
			Text.Font = GameFont.Small;
			Widgets.Label(rect1, ResourceBank.Strings.PreferedStats);
			Text.Anchor = TextAnchor.UpperLeft;
			cur.y += 5f;
			Rect rect2 = new Rect(cur.x, cur.y, canvas.width - 20f, 24f);
			if (statFilterBuffer == null)
				statFilterBuffer = "";
			if (string.IsNullOrEmpty(statFilterBuffer) && Event.current.type == EventType.Repaint)
			{
				GUI.color = Color.gray;
				Text.Anchor = TextAnchor.MiddleLeft;
				Widgets.Label(rect2.ExpandedBy(3f, 0.0f), "    filter stat...");
				Text.Anchor = TextAnchor.UpperLeft;
				GUI.color = Color.white;
			}
			statFilterBuffer = Widgets.TextField(rect2, statFilterBuffer);
			cur.y += 29f;
			Rect rect3 = new Rect(rect1.xMax - 16f, rect1.yMin + 10f, 16f, 16f);
			if (Widgets.ButtonImage(rect3, ResourceBank.Textures.AddButton))
			{
				List<FloatMenuOption> options = new List<FloatMenuOption>();
				foreach (StatDef statDef in selectedOutfit.UnassignedStats
					.Where(i => !i.alwaysHide)
					.Where(i => 
						string.IsNullOrEmpty(statFilterBuffer) || 
						i.label.ContainsIgnoreCase(statFilterBuffer) || 
						i.description.ContainsIgnoreCase(statFilterBuffer))
					.OrderBy(i => i.label)
					.OrderBy(i => i.category.displayOrder))
				{
					StatDef def = statDef;
					FloatMenuOption floatMenuOption = new FloatMenuOption(def.LabelCap, (Action)(() =>
					{
						selectedOutfit.AddStatPriority(def, 0.0f);
						guiChanged = true;
					}));
					options.Add(floatMenuOption);
				}
				Find.WindowStack.Add(new FloatMenu(options));
			}
			TooltipHandler.TipRegion(rect3, new TipSignal(ResourceBank.Strings.StatPriorityAdd));
			GUI.color = Color.grey;
			Widgets.DrawLineHorizontal(cur.x, cur.y, canvas.width);
			GUI.color = Color.white;
			cur.y += 10f;
			List<StatPriority> list = selectedOutfit.StatPriorities.ToList();
			Rect outRect = new Rect(cur.x, cur.y, canvas.width, canvas.height - cur.y);

			Rect rect4 = new Rect(outRect);
			rect4.height = 30f * list.Count;

			if (rect4.height > outRect.height)
				rect4.width -= 20f;
			Widgets.BeginScrollView(outRect, ref scrollPosition, rect4);
			GUI.BeginGroup(rect4);
			cur = Vector2.zero;
			if (list.Count > 0)
			{
				Rect rect5 = new Rect(cur.x + ((rect4.width - 24.0f) / 2.0f), cur.y, ((rect4.width - 24.0f) / 2.0f), 20f);
				Text.Font = GameFont.Tiny;
				GUI.color = Color.grey;
				Text.Anchor = TextAnchor.LowerLeft;
				Widgets.Label(rect5, "-" + 2.5f.ToString("N1"));
				Text.Anchor = TextAnchor.LowerRight;
				Widgets.Label(rect5, 2.5f.ToString("N1"));
				Text.Anchor = TextAnchor.UpperLeft;
				Text.Font = GameFont.Small;
				GUI.color = Color.white;
				cur.y += 15f;
				foreach (StatPriority statPriority in list)
					DrawStatRow(selectedOutfit, statPriority, ref cur, rect4.width);
			}
			else
			{
				Rect rect6 = new Rect(cur.x, cur.y, rect4.width, 30f);
				GUI.color = Color.grey;
				Text.Anchor = TextAnchor.MiddleCenter;
				string none = ResourceBank.Strings.None;
				Widgets.Label(rect6, none);
				Text.Anchor = TextAnchor.UpperLeft;
				GUI.color = Color.white;
				cur.y += 30f;
			}
			GUI.EndGroup();
			Widgets.EndScrollView();
		}

		private static void DrawStatRow(
		  ExtendedOutfit selectedOutfit,
		  StatPriority statPriority,
		  ref Vector2 cur,
		  float width)
		{
			Rect rect1 = new Rect(cur.x, cur.y, ((width - 24f) / 2f), 30f);
			Rect rect2 = new Rect(rect1.xMax + 4f, cur.y + 5f, rect1.width, 25f);
			Rect rect3 = new Rect(rect2.xMax + 4f, cur.y + 3f, 16f, 16f);
			Text.Font = Text.CalcHeight(statPriority.Stat.LabelCap, rect1.width) > rect1.height ? GameFont.Tiny : GameFont.Small;
			GUI.color = AssigmentColor(statPriority);
			Widgets.Label(rect1, statPriority.Stat.LabelCap);
			Text.Font = GameFont.Small;
			string text = string.Empty;
			if (statPriority.IsManual)
			{
				text = ResourceBank.Strings.StatPriorityDelete(statPriority.Stat.LabelCap);
				if (Widgets.ButtonImage(rect3, ResourceBank.Textures.DeleteButton))
				{
					selectedOutfit.RemoveStatPriority(statPriority.Stat);
					guiChanged = true;
				}
			}
			else if (statPriority.IsOverride)
			{
				text = ResourceBank.Strings.StatPriorityReset(statPriority.Stat.LabelCap);
				if (Widgets.ButtonImage(rect3, ResourceBank.Textures.ResetButton))
				{
					float weight1 = statPriority.Weight;
					statPriority.Weight = statPriority.Default;
					float weight2 = statPriority.Weight;
					if (weight1 != weight2)
						guiChanged = true;
				}
			}
			GUI.color = new Color(0.3f, 0.3f, 0.3f);
			for (var y = cur.y; y < cur.y + 30; y += 5)
				Widgets.DrawLineVertical(((rect2.xMin + rect2.xMax) / 2f), y, 3f);
			GUI.color = AssigmentColor(statPriority);
			double weight = (double)statPriority.Weight;
			float num = GUI.HorizontalSlider(rect2, statPriority.Weight, -2.5f, 2.5f);
			if (Mathf.Abs(num - statPriority.Weight) > 0.0001)
			{
				statPriority.Weight = num;
				guiChanged = true;
			}
			GUI.color = Color.white;
			TooltipHandler.TipRegion(rect1, new TipSignal(statPriority.Stat.LabelCap + "\n\n" + statPriority.Stat.description));
			if (text != string.Empty)
				TooltipHandler.TipRegion(rect3, new TipSignal(text));
			TooltipHandler.TipRegion(rect2, new TipSignal(statPriority.Weight.ToStringByStyle(ToStringStyle.FloatTwo)));
			cur.y += 30f;
		}

		private static Color AssigmentColor(StatPriority statPriority)
		{
			if (statPriority.IsManual)
				return Color.white;
			if (statPriority.IsDefault)
				return Color.grey;
			return !statPriority.IsOverride ? Color.cyan : new Color(0.75f, 0.69f, 0.33f);
		}
	}
}

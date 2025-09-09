using HarmonyLib;
using LudeonTK;
using Outfitted.Database;
using Outfitted.RW_JustUtils;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Outfitted
{
	public static class Dialog_Policies
	{
		/// Window settings.
		// From vanilla.
		public const float WinEdgeGap = 18f;
		public const float LeftPanelWidth = 200f;
		public const float GapVert = 10f;
		public const float GapHor = 10f;
		public const float WinTitleHeight = 32f;
		public const float WinTipHeight = 32f;

		// Applied through harmony patches.
		public const float WindowWidth = 900f;
		public const float WindowHeight = 700f;
		public const float MiddlePanelWidth = (WindowWidth - LeftPanelWidth - WinEdgeGap * 2 - GapVert * 2) / 2;

		// Outfitted.
		public const float RightPanelWidth = WindowWidth - LeftPanelWidth - MiddlePanelWidth - WinEdgeGap * 2 - GapVert * 2;

		public const float ButtonWidth = 150f;
		public const float ButtonHeight = 28f;
		public const float SmallGapV = 6f;

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

		public static void DrawOutfittedButtons(Rect inRect, ExtendedOutfit extendedOutfit)
		{
			Rect generatePoliciesRect = new Rect(inRect.xMax - ButtonWidth, inRect.y, ButtonWidth, ButtonHeight);
			Rect copySettingsRect = new Rect(generatePoliciesRect.x - ButtonWidth - SmallGapV, generatePoliciesRect.y, ButtonWidth, ButtonHeight);
			GenerateOutfitsButton(generatePoliciesRect);
			if (extendedOutfit != null)
				DrawCopySettingsButton(copySettingsRect, extendedOutfit);
		}

		public static void GenerateOutfitsButton(Rect inRect)
		{
			TooltipHandler.TipRegion(inRect, "Will generate missing Outfitted policies and will add missing stats to existing policies.");
			if (Widgets.ButtonText(inRect, "Generate policies"))
			{
				Find.WindowStack.Add(
					Dialog_MessageBox.CreateConfirmation(
						"This action will create missing Outfitted policies and also will extend existing policies.\n\n" +
						"No stats in existing policies will be modified or removed. Only missing stats will be added.\n\n" +
						"Filter selection WILL be modified based on apparel tag (vanilla behavior).",
						confirmedAct: () =>
						{
							StandardOutfits.EntriesFreshStart();
							StandardOutfits.GenerateStartingOutfits(Current.Game.outfitDatabase);
							Messages.Message("alrighty", MessageTypeDefOf.TaskCompletion, false);
						},
						destructive: true
					)
				);
			}
		}

		public static void DrawOutfittedTitle(Rect inRect)
		{
			using (new TextBlock(GameFont.Medium))
			{
				Widgets.Label(inRect, "Outfitted");
			}
		}

		public static void DrawOutfittedContent(Rect inRect, ExtendedOutfit extendedOutfit)
		{
			Widgets.DrawMenuSection(inRect);
			Rect mainRect = inRect.ContractedBy(10f);

			guiChanged = false;
			GUI.BeginGroup(mainRect);
			Vector2 coordinates = Vector2.zero;
			DrawDeadmanToogle(extendedOutfit, ref coordinates, mainRect);
			DrawAutoWorkPrioritiesToggle(extendedOutfit, ref coordinates, mainRect);
			DrawAutoTempToggle(extendedOutfit, ref coordinates, mainRect);
			if (!extendedOutfit.AutoTemp)
				DrawTemperatureStats(extendedOutfit, ref coordinates, mainRect);
			else
				DrawAutoTempOffsetInput(extendedOutfit, ref coordinates, mainRect);
			coordinates.y += 10f;
			DrawApparelStats(extendedOutfit, coordinates, mainRect);
			if (guiChanged && extendedOutfit != null)
				Outfitted.Notify_OutfitChanged(extendedOutfit.id);
			GUI.EndGroup();
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;
		}

		private static void DrawCopySettingsButton(Rect inRect, ExtendedOutfit selectedOutfit)
		{
			if (!Widgets.ButtonText(inRect, (string)"CommandCopyZoneSettingsLabel".Translate()))
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

		private static void DrawApparelStats(ExtendedOutfit selectedOutfit, Vector2 cur, Rect inRect)
		{
			Rect titleRect = new Rect(cur.x, cur.y, inRect.width, 22f);
			cur.y += 22f;
			Text.Anchor = TextAnchor.LowerLeft;
			Text.Font = GameFont.Small;
			Widgets.Label(titleRect, ResourceBank.Strings.PreferedStats);
			Text.Anchor = TextAnchor.UpperLeft;
			cur.y += 5f;
			Rect filterRect = new Rect(cur.x, cur.y, inRect.width - 20f, 24f);
			statFilterBuffer ??= "";
			if (string.IsNullOrEmpty(statFilterBuffer) && Event.current.type == EventType.Repaint)
			{
				GUI.color = Color.gray;
				Text.Anchor = TextAnchor.MiddleLeft;
				Widgets.Label(filterRect.ExpandedBy(3f, 0.0f), "    filter stat...");
				Text.Anchor = TextAnchor.UpperLeft;
				GUI.color = Color.white;
			}
			statFilterBuffer = Widgets.TextField(filterRect, statFilterBuffer);
			cur.y += 29f;

			// Add new stat.
			Rect addBtnRect = new Rect(titleRect.xMax - 16f, titleRect.yMin + 10f, 16f, 16f);
			TooltipHandler.TipRegion(addBtnRect, new TipSignal(ResourceBank.Strings.StatPriorityAdd));
			if (Widgets.ButtonImage(addBtnRect, ResourceBank.Textures.AddButton))
			{
				List<FloatMenuOption> options = GetStatListOptions(selectedOutfit);
				if (options.Count > 0)
					Find.WindowStack.Add(new FloatMenuPreserveOrder(options));
				else
					Messages.Message((string)"NoStatOptionsToShow".Translate(), MessageTypeDefOf.RejectInput, false);
			}

			GUI.color = Color.grey;
			Widgets.DrawLineHorizontal(cur.x, cur.y, inRect.width);
			GUI.color = Color.white;
			cur.y += 10f;

			List<StatPriority> list = selectedOutfit.StatPriorities.ToList();
			Rect scrollPosRect = new Rect(cur.x, cur.y, inRect.width, inRect.height - cur.y);
			//Utils_GUI.DrawBox(scrollPosRect, Color.green);
			Rect scrollContentRect = new Rect(scrollPosRect)
			{
				height = (list.Count > 0 ? list.Count : 1) * 30f + 16f
			};
			//Utils_GUI.DrawBox(scrollContentRect, Color.red);

			if (scrollContentRect.height > scrollPosRect.height)
				scrollContentRect.width -= 20f;
			Widgets.BeginScrollView(scrollPosRect, ref scrollPosition, scrollContentRect);
			GUI.BeginGroup(scrollContentRect);
			cur = Vector2.zero;
			if (list.Count > 0)
			{
				Rect rangeNumRect = new Rect(
					cur.x + ((scrollContentRect.width - 24.0f) / 2.0f),
					cur.y,
					((scrollContentRect.width - 24.0f) / 2.0f),
					20f);
				//Utils_GUI.DrawBox(rangeNumRect, Color.blue);
				Text.Font = GameFont.Tiny;
				GUI.color = Color.grey;
				Text.Anchor = TextAnchor.LowerLeft;
				Widgets.Label(rangeNumRect, "-" + 2.5f.ToString("N1"));
				Text.Anchor = TextAnchor.LowerRight;
				Widgets.Label(rangeNumRect, 2.5f.ToString("N1"));
				Text.Anchor = TextAnchor.UpperLeft;
				Text.Font = GameFont.Small;
				GUI.color = Color.white;
				cur.y += 15f;
				foreach (StatPriority statPriority in list)
					DrawStatRow(selectedOutfit, statPriority, ref cur, scrollContentRect.width);
			}
			else
			{
				Rect rect6 = new Rect(cur.x, cur.y, scrollContentRect.width, 30f);
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

		// Prefered order of mods displayed. All other mods will go after.
		private static readonly string[] PreferredModOrder =
		{
			"ludeon.rimworld",
			"ludeon.rimWorld.royalty",
			"ludeon.rimWorld.ideology",
			"ludeon.rimWorld.biotech",
			"ludeon.rimWorld.anomaly",
			"ludeon.rimWorld.odyssey"
		};

		// Dict for faster access.
		private static readonly Dictionary<string, int> PreferredModIndex = PreferredModOrder
			.Select((id, idx) => new { id, idx })
			.ToDictionary(x => x.id, x => x.idx, StringComparer.OrdinalIgnoreCase);

		// Get priority.
		private static int GetModPriority(Def def)
		{
			var id = def?.modContentPack?.PackageId;
			if (id != null)
				return PreferredModIndex.TryGetValue(id, out var idx) ? idx : int.MaxValue;
			else
				return int.MaxValue;
		}

		[TweakValue("00_", 0, 21)]
		static int modColor = 15;
		[TweakValue("00_", 0, 21)]
		static int catColor = 7;

		public static readonly string[] selectColor = new string[]
		{
			"aqua",
			"black",
			"blue",
			"brown",
			"cyan",
			"darkblue",
			"fuchsia",
			"green",
			"grey",
			"lightblue",
			"lime",
			"magenta",
			"maroon",
			"navy",
			"olive",
			"orange",
			"purple",
			"red",
			"silver",
			"teal",
			"white",
			"yellow"
		};

		private static List<FloatMenuOption> GetStatListOptions(ExtendedOutfit selectedOutfit)
		{
			// Filtered list.
			var filtered = selectedOutfit.UnassignedStats
				.Where(i => i != null)
				.Where(i => !i.alwaysHide)
				.Where(i =>
					string.IsNullOrEmpty(statFilterBuffer) ||
					i.label.ContainsIgnoreCase(statFilterBuffer) ||
					i.defName.ContainsIgnoreCase(statFilterBuffer) ||
					(OutfittedMod.Settings.includeDescrForStatSearch && i.description.ContainsIgnoreCase(statFilterBuffer)));

			// Final list.
			var ordered = filtered
				.OrderBy(x => GetModPriority(x))            // Pre-defined order for some mods.
				.ThenBy(x => x.modContentPack?.PackageId == null ? 1 : 0)       // Push items without modContentPack to the end
				.ThenBy(x => x.modContentPack?.PackageId ?? string.Empty)       // All other mods simple alphabetically.
				.ThenBy(x => x.category?.displayOrder ?? int.MaxValue)          // Then - by category
				.ThenBy(x => x.label ?? x.defName);         // Finally - by the label

			StatCategoryDef category = null;
			string displayedModId = null;
			string displayedModName = null;
			List<FloatMenuOption> options = new List<FloatMenuOption>();
			foreach (var item in ordered)
			{
				// Add mod name on top of each group.
				if (OutfittedMod.Settings.displayModName)
				{
					var itemModId = item.modContentPack?.PackageId ?? "<UNKNOWN>";
					var itemModName = item.modContentPack?.Name ?? "<UNKNOWN>";
#if DEBUG
					if (itemModId == "<UNKNOWN>")
						Logger.LogNL($"[DrawApparelStats]: modContentPack is null for item [{item.label ?? item.defName}].");

#endif
					if (displayedModId == null || displayedModId != itemModId)
					{
						displayedModId = itemModId;
						displayedModName = itemModName;
						options.Add(new FloatMenuOption($"<color={selectColor[modColor]}>{displayedModName}</color>", null));
						category = null;    // Reset category to display it in the next mod section.
					}
				}

				// Add category on top of each group.
				if (OutfittedMod.Settings.displayCatName)
				{
					var itemCat = item.category;
					if (itemCat != null && (category == null || category != itemCat))
					{
						category = itemCat;
						options.Add(new FloatMenuOption($"<color={selectColor[catColor]}>{category.LabelCap}</color>", null));
					}
				}

				FloatMenuOption floatMenuOption = new FloatMenuOption(
					item.LabelCap,
					() =>
					{
						selectedOutfit.AddStatPriority(item, 0.0f);
						guiChanged = true;
					});
				options.Add(floatMenuOption);
			}

			return options;
		}

		private static void DrawStatRow(
		  ExtendedOutfit selectedOutfit,
		  StatPriority statPriority,
		  ref Vector2 cur,
		  float width)
		{
			Rect rowRect = new Rect(cur.x, cur.y, width, 30f);
			//Utils_GUI.DrawBox(rowRect, Color.yellow);
			Rect labelRect = new Rect(cur.x, cur.y, ((width - 24f) / 2f), 30f);
			Rect sliderRect = new Rect(labelRect.xMax + 4f, cur.y + 5f, labelRect.width, 25f);
			Rect deleteBtnRect = new Rect(sliderRect.xMax + 4f, cur.y + 3f, 16f, 16f);
			Text.Font = Text.CalcHeight(statPriority.Stat.LabelCap, labelRect.width) > labelRect.height ? GameFont.Tiny : GameFont.Small;
			GUI.color = AssigmentColor(statPriority);
			Widgets.Label(labelRect, statPriority.Stat.LabelCap);
			Text.Font = GameFont.Small;
			string text = string.Empty;
			if (statPriority.IsManual)
			{
				text = ResourceBank.Strings.StatPriorityDelete(statPriority.Stat.LabelCap);
				if (Widgets.ButtonImage(deleteBtnRect, ResourceBank.Textures.DeleteButton))
				{
					selectedOutfit.RemoveStatPriority(statPriority.Stat);
					guiChanged = true;
				}
			}
			else if (statPriority.IsOverride)
			{
				text = ResourceBank.Strings.StatPriorityReset(statPriority.Stat.LabelCap);
				if (Widgets.ButtonImage(deleteBtnRect, ResourceBank.Textures.ResetButton))
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
				Widgets.DrawLineVertical(((sliderRect.xMin + sliderRect.xMax) / 2f), y, 3f);
			GUI.color = AssigmentColor(statPriority);
			double weight = statPriority.Weight;
			float num = GUI.HorizontalSlider(sliderRect, statPriority.Weight, -2.5f, 2.5f);
			if (Mathf.Abs(num - statPriority.Weight) > 0.0001)
			{
				statPriority.Weight = num;
				guiChanged = true;
			}
			GUI.color = Color.white;
			TooltipHandler.TipRegion(labelRect, new TipSignal(statPriority.Stat.LabelCap + "\n\n" + statPriority.Stat.description));
			if (text != string.Empty)
				TooltipHandler.TipRegion(deleteBtnRect, new TipSignal(text));
			TooltipHandler.TipRegion(sliderRect, new TipSignal(statPriority.Weight.ToStringByStyle(ToStringStyle.FloatTwo)));
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

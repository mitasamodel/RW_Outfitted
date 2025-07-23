// Decompiled with JetBrains decompiler
// Type: Outfitted.Widgets_FloatRange
// Assembly: Outfitted, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7FA0F5BF-790B-428D-866C-5D33983FFC76
// Assembly location: D:\SteamLibrary\steamapps\workshop\content\294100\3454809174\1.5\Assemblies\Outfitted.dll

using UnityEngine;
using Verse;

namespace Outfitted
{
  public static class Widgets_FloatRange
  {
    private static Widgets_FloatRange.Handle _draggingHandle;
    private static int _draggingId;

    public static void FloatRange(
      Rect canvas,
      int id,
      ref Verse.FloatRange range,
      Verse.FloatRange sliderRange,
      ToStringStyle valueStyle = ToStringStyle.Integer,
      string labelKey = null)
    {
      canvas.xMin += 8f;
      canvas.xMax -= 8f;
      Color color = GUI.color;
      GUI.color = new Color(0.4f, 0.4f, 0.4f);
      string label = range.min.ToStringByStyle(valueStyle) + " - " + range.max.ToStringByStyle(valueStyle);
      if (labelKey != null)
        label = (string) labelKey.Translate((NamedArgument) label);
      Text.Font = GameFont.Small;
      Rect rect1 = new Rect(canvas.x, canvas.y, canvas.width, 19f);
      Text.Anchor = TextAnchor.UpperCenter;
      Widgets.Label(rect1, label);
      Text.Anchor = TextAnchor.UpperLeft;
      Rect position1 = new Rect(canvas.x, rect1.yMax, canvas.width, 2f);
      GUI.DrawTexture(position1, (Texture) BaseContent.WhiteTex);
      GUI.color = color;
      float num1 = position1.width / sliderRange.Span;
      float num2 = position1.xMin + (range.min - sliderRange.min) * num1;
      float num3 = position1.xMin + (range.max - sliderRange.min) * num1;
      Rect position2 = new Rect(num2 - 16f, position1.center.y - 8f, 16f, 16f);
      GUI.DrawTexture(position2, (Texture) ResourceBank.Textures.FloatRangeSliderTex);
      Rect position3 = new Rect(num3 + 16f, position1.center.y - 8f, -16f, 16f);
      GUI.DrawTexture(position3, (Texture) ResourceBank.Textures.FloatRangeSliderTex);
      Rect rect2 = canvas;
      rect2.xMin -= 8f;
      rect2.xMax += 8f;
      bool flag = false;
      if (Mouse.IsOver(rect2) || Widgets_FloatRange._draggingId == id)
      {
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
          Widgets_FloatRange._draggingId = id;
          float x = Event.current.mousePosition.x;
          Widgets_FloatRange._draggingHandle = (double) x < (double) position2.xMax ? Widgets_FloatRange.Handle.Min : ((double) x > (double) position3.xMin ? Widgets_FloatRange.Handle.Max : ((double) Mathf.Abs(x - position2.xMax) < (double) Mathf.Abs(x - (position3.x - 16f)) ? Widgets_FloatRange.Handle.Min : Widgets_FloatRange.Handle.Max));
          flag = true;
          Event.current.Use();
        }
        if (flag || Widgets_FloatRange._draggingHandle != Widgets_FloatRange.Handle.None && Event.current.type == EventType.MouseDrag)
        {
          float num4 = Mathf.Clamp((Event.current.mousePosition.x - canvas.x) / canvas.width * sliderRange.Span + sliderRange.min, sliderRange.min, sliderRange.max);
          switch (Widgets_FloatRange._draggingHandle)
          {
            case Widgets_FloatRange.Handle.Min:
              range.min = num4;
              if ((double) range.max < (double) range.min)
              {
                range.max = range.min;
                break;
              }
              break;
            case Widgets_FloatRange.Handle.Max:
              range.max = num4;
              if ((double) range.min > (double) range.max)
              {
                range.min = range.max;
                break;
              }
              break;
          }
          Event.current.Use();
        }
      }
      if (Widgets_FloatRange._draggingHandle == Widgets_FloatRange.Handle.None || Event.current.type != EventType.MouseUp)
        return;
      Widgets_FloatRange._draggingId = 0;
      Widgets_FloatRange._draggingHandle = Widgets_FloatRange.Handle.None;
      Event.current.Use();
    }

    public enum Handle
    {
      None,
      Min,
      Max,
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Outfitted.Database.OutfitDatabase_MakeNewOutfit_Patch
// Assembly: Outfitted, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7FA0F5BF-790B-428D-866C-5D33983FFC76
// Assembly location: D:\SteamLibrary\steamapps\workshop\content\294100\3454809174\1.5\Assemblies\Outfitted.dll

using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#nullable disable
namespace Outfitted.Database
{
  [HarmonyPatch(typeof (OutfitDatabase), "MakeNewOutfit")]
  internal static class OutfitDatabase_MakeNewOutfit_Patch
  {
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
      ConstructorInfo oldConstructor = AccessTools.Constructor(typeof (ApparelPolicy), new Type[2]
      {
        typeof (int),
        typeof (string)
      }, false);
      ConstructorInfo newConstructor = AccessTools.Constructor(typeof (ExtendedOutfit), new Type[2]
      {
        typeof (int),
        typeof (string)
      }, false);
      foreach (CodeInstruction instruction in instructions)
      {
        if (instruction.opcode == OpCodes.Newobj && oldConstructor.Equals(instruction.operand))
          instruction.operand = (object) newConstructor;
        yield return instruction;
      }
    }
  }
}

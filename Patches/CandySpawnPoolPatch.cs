namespace CandyCustomizer.Patches
{
    using System;
    using System.Reflection;
    using Exiled.API.Features;
    using HarmonyLib;
    using CandyCustomizer.Runtime;

    internal static class CandySpawnPoolPatch
    {
        public static bool IsPatched { get; private set; }

        public static void Patch(Harmony harmony)
        {
            if (harmony is null)
                throw new ArgumentNullException(nameof(harmony));

            Type candiesType = AccessTools.TypeByName("InventorySystem.Items.Usables.Scp330.Scp330Candies");
            MethodInfo method = candiesType is null ? null : AccessTools.Method(candiesType, "GetRandom");

            if (method is null)
            {
                Log.Warn("Could not find Scp330Candies.GetRandom. Custom candy spawn pool control is disabled.");
                return;
            }

            HarmonyMethod prefix = new HarmonyMethod(typeof(CandySpawnPoolPatch).GetMethod(nameof(Prefix), BindingFlags.Static | BindingFlags.NonPublic));
            harmony.Patch(method, prefix);
            IsPatched = true;
        }

        private static bool Prefix(object ignoredKind, ref object __result)
        {
            if (!CandySpawnPoolSelector.TryGetRandom(ignoredKind, out object result))
                return true;

            __result = result;
            return false;
        }
    }
}

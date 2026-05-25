namespace CandyCustomizer.Patches
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Exiled.API.Features;
    using HarmonyLib;
    using CandyCustomizer.Configuration;
    using CandyCustomizer.Runtime;

    internal static class CandyEffectPatch
    {
        public static int PatchedMethodCount { get; private set; }

        public static void Patch(Harmony harmony)
        {
            if (harmony is null)
                throw new ArgumentNullException(nameof(harmony));

            Type candyInterface = AccessTools.TypeByName("InventorySystem.Items.Usables.Scp330.ICandy");

            if (candyInterface is null)
            {
                Log.Warn("Could not find InventorySystem.Items.Usables.Scp330.ICandy. Candy overrides are disabled.");
                return;
            }

            HarmonyMethod prefix = new HarmonyMethod(typeof(CandyEffectPatch).GetMethod(nameof(Prefix), BindingFlags.Static | BindingFlags.NonPublic));
            HarmonyMethod postfix = new HarmonyMethod(typeof(CandyEffectPatch).GetMethod(nameof(Postfix), BindingFlags.Static | BindingFlags.NonPublic));
            HashSet<RuntimeMethodHandle> patchedMethods = new HashSet<RuntimeMethodHandle>();
            PatchedMethodCount = 0;

            foreach (Type candyType in candyInterface.Assembly.GetTypes().Where(type => candyInterface.IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract))
            {
                MethodInfo method = candyType.GetMethod("ServerApplyEffects", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

                if (method is null || method.DeclaringType == candyInterface || !patchedMethods.Add(method.MethodHandle))
                    continue;

                try
                {
                    harmony.Patch(method, prefix, postfix);
                    PatchedMethodCount++;
                }
                catch (Exception exception)
                {
                    Log.Warn($"Could not patch SCP-330 candy effect method {method.DeclaringType?.FullName}.{method.Name}: {exception}");
                }
            }
        }

        private static bool Prefix(object __instance)
        {
            Plugin plugin = Plugin.Instance;

            if (plugin is null || __instance is null || !PendingCandyEffects.TryGetPlayer(__instance, out Player player))
                return true;

            string candyName = CandyNameResolver.Resolve(__instance);

            if (!plugin.Config.TryGetCandy(candyName, out CandySettings settings))
                return true;

            if (settings.Mode != CandyApplicationMode.OverrideVanilla)
                return true;

            CandyEffectApplier.Apply(player, settings, plugin.Config.Debug);
            PendingCandyEffects.MarkHandledByPatch(__instance);

            return false;
        }

        private static void Postfix(object __instance)
        {
            Plugin plugin = Plugin.Instance;

            if (plugin is null || __instance is null || !PendingCandyEffects.TryGetPlayer(__instance, out Player player))
                return;

            string candyName = CandyNameResolver.Resolve(__instance);

            if (!plugin.Config.TryGetCandy(candyName, out CandySettings settings))
                return;

            if (settings.Mode != CandyApplicationMode.Additive)
                return;

            CandyEffectApplier.Apply(player, settings, plugin.Config.Debug);
            PendingCandyEffects.MarkHandledByPatch(__instance);
        }
    }
}

namespace CandyCustomizer.Runtime
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Features;
    using Exiled.Loader;
    using HarmonyLib;
    using CandyCustomizer.Configuration;

    internal static class CandySpawnPoolSelector
    {
        public static bool TryGetRandom(object ignoredKind, out object result)
        {
            result = null;
            if (!TryBuildPool(ignoredKind, out Type candyKindType, out List<CandyCandidate> candidates, out float totalWeight))
                return false;

            if (totalWeight <= 0f || candidates.Count == 0)
            {
                LogDebug(Plugin.Instance?.Config.Debug == true, "custom spawn pool has no eligible candies; returning CandyKindID.None.");
                result = Enum.Parse(candyKindType, "None");
                return true;
            }

            float roll = (float)(Loader.Random.NextDouble() * totalWeight);
            float originalRoll = roll;

            foreach (CandyCandidate candidate in candidates)
            {
                roll -= candidate.Weight;

                if (roll <= 0f)
                {
                    result = candidate.Kind;
                    LogDebug(Plugin.Instance?.Config.Debug == true, $"selected {candidate.Name} from custom spawn pool with weighted roll {originalRoll:0.###}/{totalWeight:0.###}.");
                    return true;
                }
            }

            CandyCandidate fallback = candidates[candidates.Count - 1];
            result = fallback.Kind;
            LogDebug(Plugin.Instance?.Config.Debug == true, $"selected fallback spawn candy {fallback.Name} after weighted roll {originalRoll:0.###}/{totalWeight:0.###}.");
            return true;
        }

        public static bool TryDescribePool(out string description)
        {
            description = null;

            if (!TryBuildPool(ignoredKind: null, out _, out List<CandyCandidate> candidates, out float totalWeight))
            {
                description = "Candy spawn pool metadata could not be read from the current server assemblies.";
                return false;
            }

            if (candidates.Count == 0 || totalWeight <= 0f)
            {
                description = "Candy spawn pool is empty. Random SCP-330 rolls currently resolve to CandyKindID.None.";
                return true;
            }

            List<string> rows = new List<string>
            {
                $"Effective Candy Customizer spawn pool: total_weight={totalWeight:0.###}",
            };

            foreach (CandyCandidate candidate in candidates)
            {
                float chance = totalWeight <= 0f ? 0f : (candidate.Weight / totalWeight) * 100f;
                rows.Add($"{candidate.Name}: weight={candidate.Weight:0.###}, approximate_share={chance:0.##}%");
            }

            description = string.Join(Environment.NewLine, rows);
            return true;
        }

        private static bool TryBuildPool(object ignoredKind, out Type candyKindType, out List<CandyCandidate> candidates, out float totalWeight)
        {
            candyKindType = null;
            candidates = new List<CandyCandidate>();
            totalWeight = 0f;

            Plugin plugin = Plugin.Instance;

            if (plugin is null)
                return false;

            Type candiesType = AccessTools.TypeByName("InventorySystem.Items.Usables.Scp330.Scp330Candies");
            candyKindType = AccessTools.TypeByName("InventorySystem.Items.Usables.Scp330.CandyKindID");

            if (candiesType is null || candyKindType is null)
                return false;

            object candies = candiesType.GetProperty("Candies")?.GetValue(null);
            Array candyArray = candies as Array;

            if (candyArray is null || candyArray.Length == 0)
                return false;

            foreach (object candy in candyArray)
            {
                if (candy is null)
                    continue;

                object kind = candy.GetType().GetProperty("Kind")?.GetValue(candy);

                if (kind is null || Equals(kind, ignoredKind))
                    continue;

                string candyName = CandyNameResolver.Resolve(candy);
                plugin.Config.TryGetCandy(candyName, out CandySettings settings);

                float nativeWeight = ReadSpawnWeight(candy);
                float configuredWeight = settings is null || settings.SpawnWeight < 0f ? nativeWeight : settings.SpawnWeight;

                if (configuredWeight <= 0f)
                {
                    LogDebug(plugin.Config.Debug, $"{candyName} skipped because effective spawn weight is {configuredWeight}.");
                    continue;
                }

                candidates.Add(new CandyCandidate(kind, candyName, configuredWeight));
                totalWeight += configuredWeight;
            }

            return true;
        }

        private static float ReadSpawnWeight(object candy)
        {
            object value = candy.GetType().GetProperty("SpawnChanceWeight")?.GetValue(candy);
            return value is float weight ? weight : 0f;
        }

        private static void LogDebug(bool debug, string message)
        {
            if (debug)
                Log.Debug($"Candy Customizer spawn trace: {message}");
        }

        private sealed class CandyCandidate
        {
            public CandyCandidate(object kind, string name, float weight)
            {
                Kind = kind;
                Name = name;
                Weight = weight;
            }

            public object Kind { get; }

            public string Name { get; }

            public float Weight { get; }
        }
    }
}

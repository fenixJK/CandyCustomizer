namespace CandyCustomizer.Runtime
{
    using System.Collections.Generic;
    using CandyCustomizer.Configuration;
    using Exiled.API.Features;
    using Exiled.Loader;

    internal static class CandyOutcomeSelector
    {
        public static CandyBehaviorSettings Resolve(CandySettings settings, Player player, bool debug)
        {
            Dictionary<string, float> outcomes = settings?.Outcomes;

            if (outcomes is null || outcomes.Count == 0)
                return null;

            List<ResolvedOutcome> eligibleOutcomes = new List<ResolvedOutcome>();
            float totalWeight = 0f;
            int index = 0;

            foreach (KeyValuePair<string, float> reference in outcomes)
            {
                index++;

                if (reference.Value <= 0f)
                {
                    LogDebug(debug, player, $"outcome #{index} skipped: weight is {reference.Value}.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(reference.Key))
                {
                    LogDebug(debug, player, $"outcome #{index} skipped: name is blank.");
                    continue;
                }

                if (!OutcomeRepository.TryGet(reference.Key, out CandyOutcomeSettings outcome))
                {
                    LogDebug(debug, player, $"outcome #{index} skipped: outcome '{reference.Key}' was not found.");
                    continue;
                }

                if (!CandyOutcomeConditionMatcher.Matches(player, outcome.Conditions, out string rejectionReason))
                {
                    LogDebug(debug, player, $"outcome #{index} skipped by outcome conditions: {rejectionReason}.");
                    continue;
                }

                eligibleOutcomes.Add(new ResolvedOutcome(index, reference.Key, reference.Value, outcome));
                totalWeight += reference.Value;
                LogDebug(debug, player, $"outcome #{index} '{reference.Key}' eligible with weight {reference.Value}.");
            }

            if (totalWeight <= 0f)
            {
                LogDebug(debug, player, "no weighted outcome was eligible.");
                return null;
            }

            float roll = (float)(Loader.Random.NextDouble() * totalWeight);
            float originalRoll = roll;

            for (int candidateIndex = 0; candidateIndex < eligibleOutcomes.Count; candidateIndex++)
            {
                ResolvedOutcome candidate = eligibleOutcomes[candidateIndex];
                roll -= candidate.Weight;

                if (roll <= 0f)
                {
                    LogDebug(debug, player, $"selected outcome #{candidate.Index} '{candidate.Name}' with weighted roll {originalRoll:0.###}/{totalWeight:0.###}.");
                    return candidate.Outcome;
                }
            }

            ResolvedOutcome fallback = eligibleOutcomes[eligibleOutcomes.Count - 1];
            LogDebug(debug, player, $"selected final outcome #{fallback.Index} '{fallback.Name}' after weighted roll {originalRoll:0.###}/{totalWeight:0.###}.");
            return fallback.Outcome;
        }

        private static void LogDebug(bool debug, Player player, string message)
        {
            if (debug)
                Log.Debug($"Candy Customizer outcome trace for {player?.Nickname ?? "unknown player"}: {message}");
        }

        private sealed class ResolvedOutcome
        {
            public ResolvedOutcome(int index, string name, float weight, CandyOutcomeSettings outcome)
            {
                Index = index;
                Name = name;
                Weight = weight;
                Outcome = outcome;
            }

            public int Index { get; }

            public string Name { get; }

            public float Weight { get; }

            public CandyOutcomeSettings Outcome { get; }
        }
    }
}

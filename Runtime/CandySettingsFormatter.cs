namespace CandyCustomizer.Runtime
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using CandyCustomizer.Configuration;

    internal static class CandySettingsFormatter
    {
        public static string Format(string candyName, CandySettings settings)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(candyName)
                .Append(": mode=").Append(settings.Mode)
                .Append(", can_eat=").Append(settings.CanEat)
                .Append(", spawn_weight=").Append(settings.SpawnWeight)
                .Append(", outcomes=").Append(settings.Outcomes?.Count ?? 0);

            if (settings.Outcomes is not null && settings.Outcomes.Count > 0)
            {
                int index = 0;

                foreach (KeyValuePair<string, float> outcome in settings.Outcomes)
                {
                    index++;
                    builder.AppendLine()
                        .Append("  #").Append(index).Append(": ")
                        .Append("name=").Append(string.IsNullOrWhiteSpace(outcome.Key) ? "<blank>" : outcome.Key)
                        .Append(", weight=").Append(outcome.Value);
                }

                return builder.ToString();
            }

            builder.Append(", no outcome weights configured");
            return builder.ToString();
        }

        public static string FormatOutcome(string outcomeName, CandyOutcomeSettings outcome)
        {
            return $"{outcomeName}: {FormatPayload(outcome)}, max_health={outcome.MaxHealth}, max_ahp={outcome.MaxArtificialHealth}, kill_reason={(string.IsNullOrWhiteSpace(outcome.KillReason) ? "none" : outcome.KillReason)}, hint_duration={outcome.HintDuration}, {FormatConditions(outcome.Conditions)}";
        }

        private static string FormatPayload(CandyBehaviorSettings settings)
        {
            return $"effects={FormatEffects(settings.Effects)}, health={settings.Health}, ahp={settings.ArtificialHealth}, kill={settings.Kill}, explode={settings.Explode}, hint={(string.IsNullOrWhiteSpace(settings.Hint) ? "none" : "set")}";
        }

        private static string FormatConditions(CandyOutcomeConditions conditions)
        {
            if (conditions is null)
                return "conditions=none";

            return $"conditions=roles[{FormatList(conditions.AllowedRoles)}]/deny[{FormatList(conditions.DeniedRoles)}], teams[{FormatList(conditions.AllowedTeams)}]/deny[{FormatList(conditions.DeniedTeams)}], round={conditions.MinRoundElapsedSeconds}..{conditions.MaxRoundElapsedSeconds}s";
        }

        private static string FormatEffects(IEnumerable<EffectSettings> effects)
        {
            if (effects is null)
                return "none";

            string value = string.Join(",", effects.Where(effect => effect is not null && !string.IsNullOrWhiteSpace(effect.Name)).Select(effect => effect.Name));
            return string.IsNullOrWhiteSpace(value) ? "none" : value;
        }

        private static string FormatList(IEnumerable<string> values)
        {
            if (values is null)
                return "*";

            string value = string.Join(",", values.Where(item => !string.IsNullOrWhiteSpace(item)));
            return string.IsNullOrWhiteSpace(value) ? "*" : value;
        }
    }
}

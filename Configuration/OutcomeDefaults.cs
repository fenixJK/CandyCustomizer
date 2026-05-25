namespace CandyCustomizer.Configuration
{
    using System.Collections.Generic;

    internal static class OutcomeDefaults
    {
        public static OutcomeConfiguration Create()
        {
            OutcomeConfiguration configuration = new OutcomeConfiguration();

            configuration.Outcomes["complete_example"] = new CandyOutcomeSettings
            {
                Effects =
                {
                    new EffectSettings
                    {
                        Name = "DamageReduction",
                        Intensity = 1,
                        Duration = 12f,
                    },
                },
                Health = 20f,
                MaxHealth = 120f,
                ArtificialHealth = 25f,
                MaxArtificialHealth = 75f,
                Kill = false,
                KillReason = "A strange candy",
                Explode = false,
                Hint = "Conditional defensive outcome example.",
                HintDuration = 3f,
                Conditions = new CandyOutcomeConditions
                {
                    AllowedRoles = new List<string>(),
                    DeniedRoles = new List<string>(),
                    AllowedTeams = new List<string> { "ClassD", "Scientists", "FoundationForces" },
                    DeniedTeams = new List<string>(),
                    MaxRoundElapsedSeconds = 420f,
                },
            };

            configuration.Outcomes["risky_bite"] = new CandyOutcomeSettings
            {
                Health = -20f,
                KillReason = "An unstable candy",
                Hint = "Conditional risky outcome example.",
            };

            return configuration;
        }
    }
}

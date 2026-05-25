namespace CandyCustomizer.Configuration
{
    using System.Collections.Generic;

    public sealed class CandyOutcomeConditions
    {
        public List<string> AllowedRoles { get; set; }

        public List<string> DeniedRoles { get; set; }

        public List<string> AllowedTeams { get; set; }

        public List<string> DeniedTeams { get; set; }

        public float MinRoundElapsedSeconds { get; set; } = -1f;

        public float MaxRoundElapsedSeconds { get; set; } = -1f;
    }
}

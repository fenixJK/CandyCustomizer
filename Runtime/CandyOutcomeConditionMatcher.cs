namespace CandyCustomizer.Runtime
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Features;
    using CandyCustomizer.Configuration;

    internal static class CandyOutcomeConditionMatcher
    {
        public static bool Matches(Player player, CandyOutcomeConditions conditions, out string rejectionReason)
        {
            rejectionReason = null;

            if (player is null || conditions is null)
                return true;

            string role = player.Role?.Type.ToString() ?? string.Empty;
            string team = player.Role?.Team.ToString() ?? string.Empty;

            if (!MatchesAllowList(role, conditions.AllowedRoles) || MatchesDenyList(role, conditions.DeniedRoles))
            {
                rejectionReason = $"role '{role}' did not pass role filters";
                return false;
            }

            if (!MatchesAllowList(team, conditions.AllowedTeams) || MatchesDenyList(team, conditions.DeniedTeams))
            {
                rejectionReason = $"team '{team}' did not pass team filters";
                return false;
            }

            float elapsedSeconds = (float)Round.ElapsedTime.TotalSeconds;

            if (conditions.MinRoundElapsedSeconds >= 0f && elapsedSeconds < conditions.MinRoundElapsedSeconds)
            {
                rejectionReason = $"round elapsed {elapsedSeconds:0.###}s is below min_round_elapsed_seconds {conditions.MinRoundElapsedSeconds:0.###}";
                return false;
            }

            if (conditions.MaxRoundElapsedSeconds >= 0f && elapsedSeconds > conditions.MaxRoundElapsedSeconds)
            {
                rejectionReason = $"round elapsed {elapsedSeconds:0.###}s is above max_round_elapsed_seconds {conditions.MaxRoundElapsedSeconds:0.###}";
                return false;
            }

            return true;
        }

        private static bool MatchesAllowList(string value, IReadOnlyCollection<string> values)
        {
            return values is null || values.Count == 0 || Contains(values, value);
        }

        private static bool MatchesDenyList(string value, IReadOnlyCollection<string> values)
        {
            return values is not null && values.Count > 0 && Contains(values, value);
        }

        private static bool Contains(IEnumerable<string> values, string expected)
        {
            if (string.IsNullOrWhiteSpace(expected))
                return false;

            foreach (string value in values)
            {
                if (string.Equals(value, expected, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
    }
}

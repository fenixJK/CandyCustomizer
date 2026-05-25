namespace CandyCustomizer.Configuration
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Enums;
    using CandyCustomizer.Metadata;
    using PlayerRoles;

    internal static class ConfigValidator
    {
        public static ConfigValidationResult Validate(Config config, OutcomeConfiguration outcomes)
        {
            ConfigValidationResult result = new ConfigValidationResult();

            if (config?.Candies is null)
            {
                result.Warnings.Add("candies is null. The plugin will not process any candy overrides.");
                return result;
            }

            foreach (KeyValuePair<string, CandySettings> pair in config.Candies)
            {
                string candyName = pair.Key;
                CandySettings settings = pair.Value;

                if (!CandyMetadataRegistry.IsKnownCandy(candyName))
                    result.Warnings.Add($"candies.{candyName}: unknown candy key. This entry will never match a supported SCP-330 candy.");

                if (settings is null)
                {
                    result.Warnings.Add($"candies.{candyName}: entry is null and will be ignored.");
                    continue;
                }

                ValidateCandy(candyName, settings, outcomes, result);
            }

            ValidateOutcomeDefinitions(outcomes, result);
            return result;
        }

        private static void ValidateCandy(string candyName, CandySettings settings, OutcomeConfiguration outcomes, ConfigValidationResult result)
        {
            if (settings.SpawnWeight < -1f)
                result.Warnings.Add($"candies.{candyName}.spawn_weight: values below -1 are invalid. Use -1 for native game weight.");

            if (settings.Mode == CandyApplicationMode.VanillaOnly && settings.Outcomes?.Count > 0)
                result.Warnings.Add($"candies.{candyName}: custom outcomes are configured, but mode is VanillaOnly. Set mode to Additive or OverrideVanilla for custom behavior to run.");

            ValidateOutcomeReferences(candyName, settings, outcomes, result);
        }

        private static void ValidateOutcomeReferences(string candyName, CandySettings settings, OutcomeConfiguration outcomes, ConfigValidationResult result)
        {
            if (settings.Outcomes is null)
            {
                result.Warnings.Add($"candies.{candyName}.outcomes: null map will be treated as disabled.");
                return;
            }

            if (settings.Outcomes.Count == 0)
                return;

            float positiveWeight = 0f;

            foreach (KeyValuePair<string, float> outcome in settings.Outcomes)
            {
                string path = $"candies.{candyName}.outcomes.{outcome.Key}";

                if (string.IsNullOrWhiteSpace(outcome.Key))
                    result.Warnings.Add($"candies.{candyName}.outcomes: blank outcome name will be ignored.");
                else if (!HasOutcome(outcomes, outcome.Key))
                    result.Warnings.Add($"{path}: outcome '{outcome.Key}' was not found in outcomes.yml.");

                if (outcome.Value <= 0f)
                    result.Warnings.Add($"{path}: non-positive outcome weights are never selected.");
                else
                    positiveWeight += outcome.Value;
            }

            if (positiveWeight <= 0f)
                result.Warnings.Add($"candies.{candyName}.outcomes: no positive-weight outcome can be selected.");
        }

        private static void ValidateOutcomeDefinitions(OutcomeConfiguration outcomes, ConfigValidationResult result)
        {
            if (outcomes?.Outcomes is null)
            {
                result.Warnings.Add("outcomes.yml outcomes is null. Named outcomes cannot be used.");
                return;
            }

            foreach (KeyValuePair<string, CandyOutcomeSettings> pair in outcomes.Outcomes)
            {
                string outcomeName = pair.Key;
                CandyOutcomeSettings outcome = pair.Value;

                if (string.IsNullOrWhiteSpace(outcomeName))
                {
                    result.Warnings.Add("outcomes.yml contains a blank outcome name.");
                    continue;
                }

                if (outcome is null)
                {
                    result.Warnings.Add($"outcomes.yml outcomes.{outcomeName}: null outcome payload will be ignored.");
                    continue;
                }

                ValidateBehavior($"outcomes.yml outcomes.{outcomeName}", outcome, result);
                ValidateConditions($"outcomes.yml outcomes.{outcomeName}", outcome.Conditions, result);
            }
        }

        private static void ValidateBehavior(string pathPrefix, CandyBehaviorSettings settings, ConfigValidationResult result)
        {
            if (settings.HintDuration < 0f)
                result.Warnings.Add($"{pathPrefix}.hint_duration: negative values are invalid for display timing.");

            if (settings.Kill && settings.Explode)
                result.Warnings.Add($"{pathPrefix}: kill and explode are both enabled. Explosion resolves first, then explicit kill only runs if the player survives.");

            if ((settings.Kill || settings.Explode || settings.Health < 0f) && string.IsNullOrWhiteSpace(settings.KillReason))
                result.Warnings.Add($"{pathPrefix}.kill_reason: blank reason falls back to generic handling for lethal direct damage.");

            if (settings.Effects is null)
            {
                result.Warnings.Add($"{pathPrefix}.effects: null list will be treated as empty.");
                return;
            }

            for (int index = 0; index < settings.Effects.Count; index++)
            {
                EffectSettings effect = settings.Effects[index];
                string path = $"{pathPrefix}.effects[{index}]";

                if (effect is null)
                {
                    result.Warnings.Add($"{path}: null effect entry will be ignored.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(effect.Name))
                {
                    result.Warnings.Add($"{path}.name: blank effect name will be ignored.");
                    continue;
                }

                if (!Enum.TryParse(effect.Name, true, out EffectType _))
                    result.Warnings.Add($"{path}.name: '{effect.Name}' is not a known EXILED 9.13.3 effect enum value.");

                if (effect.Duration < 0f)
                    result.Warnings.Add($"{path}.duration: negative durations are invalid. Use 0 for effects that support infinite duration.");
            }
        }

        private static bool HasOutcome(OutcomeConfiguration outcomes, string outcomeName)
        {
            if (outcomes?.Outcomes is null || string.IsNullOrWhiteSpace(outcomeName))
                return false;

            foreach (string configuredOutcome in outcomes.Outcomes.Keys)
            {
                if (string.Equals(configuredOutcome, outcomeName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        private static void ValidateConditions(string path, CandyOutcomeConditions conditions, ConfigValidationResult result)
        {
            if (conditions is null)
                return;

            if (conditions.MinRoundElapsedSeconds >= 0f && conditions.MaxRoundElapsedSeconds >= 0f && conditions.MinRoundElapsedSeconds > conditions.MaxRoundElapsedSeconds)
                result.Warnings.Add($"{path}.conditions: min_round_elapsed_seconds is greater than max_round_elapsed_seconds, so this outcome cannot match.");

            ValidateEnumList<RoleTypeId>(conditions.AllowedRoles, $"{path}.conditions.allowed_roles", result);
            ValidateEnumList<RoleTypeId>(conditions.DeniedRoles, $"{path}.conditions.denied_roles", result);
            ValidateEnumList<Team>(conditions.AllowedTeams, $"{path}.conditions.allowed_teams", result);
            ValidateEnumList<Team>(conditions.DeniedTeams, $"{path}.conditions.denied_teams", result);
        }

        private static void ValidateEnumList<TEnum>(IEnumerable<string> values, string path, ConfigValidationResult result)
            where TEnum : struct
        {
            if (values is null)
                return;

            foreach (string value in values)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    result.Warnings.Add($"{path}: blank value will never match.");
                    continue;
                }

                if (!Enum.TryParse(value, true, out TEnum _))
                    result.Warnings.Add($"{path}: '{value}' is not a valid {typeof(TEnum).Name} value.");
            }
        }
    }
}

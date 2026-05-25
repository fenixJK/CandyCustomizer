namespace CandyCustomizer.Metadata
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Enums;

    internal static class CandyMetadataRegistry
    {
        private static readonly string[] CandyNamesInternal =
        {
            "Rainbow",
            "Yellow",
            "Purple",
            "Red",
            "Green",
            "Blue",
            "Pink",
            "Orange",
            "White",
            "Gray",
            "Black",
            "Brown",
            "Evil",
        };

        private static readonly Dictionary<string, DamageType> LethalEffectDamageTypes =
            new Dictionary<string, DamageType>(StringComparer.OrdinalIgnoreCase)
            {
                ["Asphyxiated"] = DamageType.Asphyxiation,
                ["Bleeding"] = DamageType.Bleeding,
                ["CardiacArrest"] = DamageType.CardiacArrest,
                ["Corroding"] = DamageType.PocketDimension,
                ["Hemorrhage"] = DamageType.Bleeding,
                ["Hypothermia"] = DamageType.Hypothermia,
                ["PitDeath"] = DamageType.Crushed,
                ["PocketCorroding"] = DamageType.PocketDimension,
                ["Poisoned"] = DamageType.Poison,
                ["Scp207"] = DamageType.Scp207,
                ["Strangled"] = DamageType.Strangled,
            };

        public static IReadOnlyList<string> CandyNames => CandyNamesInternal;

        public static bool IsKnownCandy(string candyName)
        {
            if (string.IsNullOrWhiteSpace(candyName))
                return false;

            foreach (string knownCandy in CandyNamesInternal)
            {
                if (string.Equals(knownCandy, candyName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        public static string FindCandyName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            foreach (string candyName in CandyNamesInternal)
            {
                if (value.IndexOf(candyName, StringComparison.OrdinalIgnoreCase) >= 0)
                    return candyName;
            }

            return string.Empty;
        }

        public static bool TryGetLethalEffectDamageType(string effectName, out DamageType damageType)
        {
            damageType = default;
            return !string.IsNullOrWhiteSpace(effectName) && LethalEffectDamageTypes.TryGetValue(effectName, out damageType);
        }
    }
}

namespace CandyCustomizer.Configuration
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Exiled.API.Interfaces;

    public sealed class Config : IConfig
    {
        [Description("Whether this plugin is enabled.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Whether debug messages should be printed to the server console.")]
        public bool Debug { get; set; } = false;

        [Description("Candy behavior by candy name. This file controls modes, eating, spawn weights, and outcome weights. Edit outcomes.yml for the actual effects, health changes, conditions, hints, explosions, and kill settings. Valid names: Rainbow, Yellow, Purple, Red, Green, Blue, Pink, Orange, White, Gray, Black, Brown, Evil.")]
        public Dictionary<string, CandySettings> Candies { get; set; } = CandyDefaults.Create();

        public bool TryGetCandy(string candyName, out CandySettings settings)
        {
            settings = null;

            if (string.IsNullOrWhiteSpace(candyName) || Candies is null)
                return false;

            foreach (KeyValuePair<string, CandySettings> pair in Candies)
            {
                if (string.Equals(pair.Key, candyName, System.StringComparison.OrdinalIgnoreCase))
                {
                    settings = pair.Value;
                    return settings is not null;
                }
            }

            return false;
        }
    }
}

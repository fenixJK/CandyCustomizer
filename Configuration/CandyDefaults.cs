namespace CandyCustomizer.Configuration
{
    using System.Collections.Generic;
    using CandyCustomizer.Metadata;

    internal static class CandyDefaults
    {
        public static Dictionary<string, CandySettings> Create()
        {
            Dictionary<string, CandySettings> candies = new Dictionary<string, CandySettings>();

            foreach (string candy in CandyMetadataRegistry.CandyNames)
                candies[candy] = new CandySettings();

            candies["Rainbow"] = new DocumentedCandySettings();

            candies["Rainbow"].Outcomes["complete_example"] = 3f;
            candies["Rainbow"].Outcomes["risky_bite"] = 1f;

            return candies;
        }
    }
}

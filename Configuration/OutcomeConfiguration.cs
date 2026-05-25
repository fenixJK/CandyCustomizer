namespace CandyCustomizer.Configuration
{
    using System.Collections.Generic;

    public sealed class OutcomeConfiguration
    {
        public Dictionary<string, CandyOutcomeSettings> Outcomes { get; set; } =
            new Dictionary<string, CandyOutcomeSettings>();
    }
}

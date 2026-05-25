namespace CandyCustomizer.Configuration
{
    using System.Collections.Generic;

    internal sealed class ConfigValidationResult
    {
        public List<string> Warnings { get; } = new List<string>();

        public bool HasWarnings => Warnings.Count > 0;
    }
}

namespace CandyCustomizer.Configuration
{
    public sealed class EffectSettings
    {
        public string Name { get; set; } = "Vitality";

        public byte Intensity { get; set; } = 1;

        public float Duration { get; set; } = 15f;

        public bool AddDurationIfActive { get; set; } = true;
    }
}

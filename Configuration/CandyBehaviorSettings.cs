namespace CandyCustomizer.Configuration
{
    using System.Collections.Generic;

    public class CandyBehaviorSettings
    {
        public virtual List<EffectSettings> Effects { get; set; } = new List<EffectSettings>();

        public virtual float Health { get; set; } = 0f;

        public virtual float MaxHealth { get; set; } = -1f;

        public virtual float ArtificialHealth { get; set; } = 0f;

        public virtual float MaxArtificialHealth { get; set; } = -1f;

        public virtual bool Kill { get; set; } = false;

        public virtual string KillReason { get; set; } = "A strange candy";

        public virtual bool Explode { get; set; } = false;

        public virtual string Hint { get; set; } = string.Empty;

        public virtual float HintDuration { get; set; } = 3f;
    }
}

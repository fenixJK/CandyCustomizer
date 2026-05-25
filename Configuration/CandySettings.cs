namespace CandyCustomizer.Configuration
{
    using System.Collections.Generic;

    public class CandySettings
    {
        public virtual CandyApplicationMode Mode { get; set; } = CandyApplicationMode.VanillaOnly;

        public virtual bool CanEat { get; set; } = true;

        public virtual float SpawnWeight { get; set; } = -1f;

        public virtual Dictionary<string, float> Outcomes { get; set; } = new Dictionary<string, float>();
    }
}

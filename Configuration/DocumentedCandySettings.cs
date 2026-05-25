namespace CandyCustomizer.Configuration
{
    using System.Collections.Generic;
    using System.ComponentModel;

    internal sealed class DocumentedCandySettings : CandySettings
    {
        [Description("VanillaOnly leaves the candy unchanged, Additive applies these settings after vanilla, OverrideVanilla skips vanilla effects and applies only these settings.")]
        public override CandyApplicationMode Mode
        {
            get => base.Mode;
            set => base.Mode = value;
        }

        [Description("If false, eating is blocked when this candy uses Additive or OverrideVanilla mode. Use spawn_weight to control whether a candy appears randomly.")]
        public override bool CanEat
        {
            get => base.CanEat;
            set => base.CanEat = value;
        }

        [Description("Random SCP-330 pool weight. -1 uses the game's native candy weight. 0 removes this candy from random pool rolls.")]
        public override float SpawnWeight
        {
            get => base.SpawnWeight;
            set => base.SpawnWeight = value;
        }

        [Description("Weighted outcome choices from outcomes.yml. Format: outcome_name: weight. One condition-matching outcome is selected by weight.")]
        public override Dictionary<string, float> Outcomes
        {
            get => base.Outcomes;
            set => base.Outcomes = value;
        }
    }
}

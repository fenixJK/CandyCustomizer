namespace CandyCustomizer.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CandyCustomizer.Metadata;
    using Exiled.API.Enums;
    using PlayerRoles;

    public sealed class ConfigurationManifest
    {
        public Dictionary<string, string> Overview { get; set; } = new Dictionary<string, string>
        {
            ["purpose"] = "Candy Customizer reference manifest. This file is generated for operators; editing it does not change gameplay.",
            ["config"] = "Edit the Candy Customizer config file plus outcomes.yml when using named reusable outcomes, then run candyreload.",
            ["example"] = "The first generated candy entry shows outcome weights, while outcomes.yml stores the actual outcomes and conditions.",
        };

        public Dictionary<string, string> Modes { get; set; } = new Dictionary<string, string>
        {
            ["VanillaOnly"] = "Normal candy. Custom fields are ignored.",
            ["Additive"] = "Normal candy plus your custom fields.",
            ["OverrideVanilla"] = "Skip normal effects; use your fields.",
        };

        public List<string> CandyNames { get; set; } = CandyMetadataRegistry.CandyNames.ToList();

        public List<string> RoleNames { get; set; } = Enum.GetNames(typeof(RoleTypeId)).ToList();

        public List<string> TeamNames { get; set; } = Enum.GetNames(typeof(Team)).ToList();

        public Dictionary<string, string> Fields { get; set; } = new Dictionary<string, string>
        {
            ["mode"] = "VanillaOnly, Additive, or OverrideVanilla.",
            ["can_eat"] = "If false, eating is blocked when mode is Additive or OverrideVanilla. Use spawn_weight to control random availability.",
            ["spawn_weight"] = "Random pool weight. -1 uses the game's native candy weight. 0 removes the candy from random pool rolls.",
            ["outcomes"] = "Weighted outcome map. Format: outcome_name: weight. One positive-weight outcome from outcomes.yml is selected after conditions pass.",
            ["outcomes.yml.conditions.allowed_roles"] = "Optional role names that may use this outcome.",
            ["outcomes.yml.conditions.denied_roles"] = "Optional role names that may not use this outcome.",
            ["outcomes.yml.conditions.allowed_teams"] = "Optional team names that may use this outcome.",
            ["outcomes.yml.conditions.denied_teams"] = "Optional team names that may not use this outcome.",
            ["outcomes.yml.conditions.min_round_elapsed_seconds"] = "Minimum round elapsed seconds required. Omit to disable this check.",
            ["outcomes.yml.conditions.max_round_elapsed_seconds"] = "Maximum round elapsed seconds allowed. Omit to disable this check.",
            ["effects"] = "List of status effects.",
            ["effects.name"] = "Effect name from effects.",
            ["effects.intensity"] = "Effect strength/amplification.",
            ["effects.duration"] = "Duration in seconds.",
            ["effects.add_duration_if_active"] = "Extends active effect time.",
            ["health"] = "Instant HP. Positive heals; negative hurts.",
            ["max_health"] = "HP cap. -1 means unchanged.",
            ["artificial_health"] = "Instant AHP. Positive adds; negative removes.",
            ["max_artificial_health"] = "AHP cap. -1 means unchanged.",
            ["kill"] = "Kills after applying fields.",
            ["kill_reason"] = "Death reason for explicit kills, lethal negative health, tracked lethal candy effects, and tracked eater explosions.",
            ["explode"] = "Triggers Player.Explode.",
            ["hint"] = "Shown after custom behavior is applied.",
            ["hint_duration"] = "Hint seconds.",
        };

        public List<string> UsageNotes { get; set; } = new List<string>
        {
            "Effects are YAML list items under effects.",
            "Use effects: [] for no custom effects.",
            "The documented first config candy entry shows weighted outcome references.",
            "Use outcomes.yml for reusable outcomes and conditions. Keep outcome weights in the main config to decide what each candy can roll.",
            "Outcome conditions are checked before weights are rolled.",
            "Health is instant. Use negative health instead of a separate damage field. Vitality/Regen are effects.",
            "Tracked effect death reasons cover candy-applied Asphyxiated, Bleeding/Hemorrhage, CardiacArrest, Corroding/PocketCorroding, Hypothermia, PitDeath, Poisoned, Scp207, and Strangled.",
        };

        public Dictionary<string, string> ReloadCommand { get; set; } = new Dictionary<string, string>
        {
            ["command"] = "candyreload",
            ["aliases"] = "ccreload, reloadcandy",
            ["permission"] = "candycustomizer.reload",
            ["scope"] = "Reloads only Candy Customizer config.",
            ["when_to_use"] = "Run after saving config changes.",
        };

        public Dictionary<string, string> DiagnosticsCommands { get; set; } = new Dictionary<string, string>
        {
            ["inspect"] = "candyinspect <candy> | aliases: ccinspect, inspectcandy | permission: candycustomizer.inspect",
            ["pool"] = "candypool | aliases: candyweights, ccpool | permission: candycustomizer.inspect",
            ["outcome"] = "outcomeinspect <name> | aliases: ccoutcome, inspectoutcome | permission: candycustomizer.inspect",
            ["test_self"] = "candytest <candy> | aliases: cctest, testcandy | permission: candycustomizer.test",
            ["test_target"] = "candytest <candy> <player-id-or-name> lets Remote Admin or server console target another player.",
            ["test_scope"] = "candytest applies only the plugin's configured custom behavior. It does not simulate vanilla SCP:SL candy behavior.",
        };

        public Dictionary<string, string> OutcomeRules { get; set; } = new Dictionary<string, string>
        {
            ["weighted_rainbow"] = "Rainbow.outcomes demonstrates weighted outcome selection. outcomes.yml demonstrates condition checks.",
            ["selection_rule"] = "Only outcomes with weight > 0 participate.",
            ["condition_rule"] = "An outcome must pass every configured condition before it can be weighted.",
            ["fallback"] = "If outcomes is empty or no outcome is eligible, no custom outcome is applied.",
        };

        public Dictionary<string, string> SpawnPoolRules { get; set; } = new Dictionary<string, string>
        {
            ["scope"] = "Spawn pool settings affect random SCP-330 candy rolls such as bowl takes and random bag fills.",
            ["native_weight"] = "spawn_weight: -1 keeps the game's native SpawnChanceWeight for that candy.",
            ["disable"] = "spawn_weight: 0 removes the candy from the custom random pool.",
            ["empty_pool"] = "If every eligible candy resolves to weight 0, the plugin returns CandyKindID.None for that random roll.",
        };

        public Dictionary<string, string> Effects { get; set; } = new Dictionary<string, string>
        {
            ["AmnesiaItems"] = "Prevents or disrupts item usage while active.",
            ["AmnesiaVision"] = "Adds the visual amnesia effect and disrupts normal vision.",
            ["Asphyxiated"] = "Applies asphyxiation pressure, usually damaging or threatening the player over time.",
            ["Bleeding"] = "Makes the player bleed and lose health over time.",
            ["Blinded"] = "Darkens or blocks the player's vision.",
            ["Burned"] = "Applies burn damage or burn-related visual feedback.",
            ["Concussed"] = "Disorients the player and affects vision or control.",
            ["Corroding"] = "Applies corrosion damage over time.",
            ["Deafened"] = "Reduces or disrupts the player's hearing.",
            ["Decontaminating"] = "Applies the decontamination damage effect.",
            ["Disabled"] = "Disables or heavily restricts the player.",
            ["Ensnared"] = "Restricts movement, similar to being trapped or held.",
            ["Exhausted"] = "Limits stamina recovery or makes sprinting harder.",
            ["Flashed"] = "Applies flashbang-style blindness/disorientation.",
            ["Hemorrhage"] = "Applies serious bleeding-style health loss.",
            ["Invigorated"] = "Improves stamina or stamina recovery.",
            ["BodyshotReduction"] = "Reduces body-shot damage taken.",
            ["Poisoned"] = "Poisons the player and drains health over time.",
            ["Scp207"] = "Applies SCP-207 cola behavior, usually speed boost with health drain over time.",
            ["Invisible"] = "Makes the player invisible or harder to see while active.",
            ["SinkHole"] = "Slows or drags the player as if affected by a sinkhole.",
            ["DamageReduction"] = "Reduces damage taken. Higher intensity usually means stronger reduction.",
            ["MovementBoost"] = "Increases movement speed. Intensity controls boost strength.",
            ["RainbowTaste"] = "Applies the rainbow candy visual/status effect.",
            ["SeveredHands"] = "Prevents normal hand/item interaction.",
            ["Stained"] = "Marks the player with the stained effect.",
            ["Vitality"] = "Improves survivability, commonly used for resistance/healing-style candy effects.",
            ["Hypothermia"] = "Applies cold/hypothermia penalties.",
            ["Scp1853"] = "Applies SCP-1853 behavior, affecting movement/item handling with its normal drawbacks.",
            ["CardiacArrest"] = "Applies cardiac arrest pressure and can become lethal.",
            ["InsufficientLighting"] = "Applies low-light related visual effects.",
            ["SoundtrackMute"] = "Mutes or changes soundtrack behavior for the player.",
            ["SpawnProtected"] = "Gives spawn protection-style damage protection.",
            ["Traumatized"] = "Applies trauma visual/status penalties.",
            ["AntiScp207"] = "Applies Anti-SCP-207 behavior, generally countering SCP-207 style effects.",
            ["Scanned"] = "Marks the player as scanned/detected.",
            ["PocketCorroding"] = "Applies corrosion associated with the pocket dimension.",
            ["SilentWalk"] = "Makes the player's footsteps quieter or silent.",
            ["Marshmallow"] = "Applies marshmallow candy behavior/status.",
            ["Strangled"] = "Restricts breathing or applies strangling pressure.",
            ["Ghostly"] = "Applies ghostly candy behavior/status.",
            ["FogControl"] = "Applies fog-control related status behavior.",
            ["Slowness"] = "Reduces player movement speed.",
            ["Scp1344"] = "Applies SCP-1344 behavior/status.",
            ["SeveredEyes"] = "Applies severe vision impairment.",
            ["PitDeath"] = "Applies pit-death related status behavior.",
            ["Blurred"] = "Blurs or distorts player vision.",
            ["Scp1344Detected"] = "Marks the player as detected by SCP-1344 behavior.",
            ["Scp1576"] = "Applies SCP-1576 behavior/status.",
            ["Lightweight"] = "Applies lightweight movement/status behavior.",
            ["HeavyFooted"] = "Makes movement heavier or noisier depending on game behavior.",
            ["Fade"] = "Applies fade visual/status behavior.",
            ["NightVision"] = "Gives night vision or improved dark-area visibility.",
            ["Scp1509Resurrected"] = "Marks or handles SCP-1509 resurrection status.",
            ["FocusedVision"] = "Applies focused vision behavior/status.",
            ["AnomalousRegeneration"] = "Applies anomalous regeneration behavior/status.",
            ["AnomalousTarget"] = "Marks the player as an anomalous target.",
            ["BecomingFlamingo"] = "Seasonal effect. Only works when the game build/event supports it.",
            ["Scp559"] = "Seasonal effect. Only works when the game build/event supports it.",
            ["Scp956Target"] = "Seasonal effect. Only works when the game build/event supports it.",
            ["Snowed"] = "Seasonal effect. Only works when the game build/event supports it.",
            ["Metal"] = "Seasonal Halloween effect. Only works when the game build/event supports it.",
            ["OrangeCandy"] = "Seasonal Halloween effect. Only works when the game build/event supports it.",
            ["OrangeWitness"] = "Seasonal Halloween effect. Only works when the game build/event supports it.",
            ["Prismatic"] = "Seasonal Halloween effect. Only works when the game build/event supports it.",
            ["SlowMetabolism"] = "Seasonal Halloween effect. Only works when the game build/event supports it.",
            ["Spicy"] = "Seasonal Halloween effect. Only works when the game build/event supports it.",
            ["SugarCrave"] = "Seasonal Halloween or Christmas effect. Only works when the game build/event supports it.",
            ["SugarHigh"] = "Seasonal Halloween effect. Only works when the game build/event supports it.",
            ["SugarRush"] = "Seasonal Halloween effect. Only works when the game build/event supports it.",
            ["TemporaryBypass"] = "Seasonal Halloween effect. Only works when the game build/event supports it.",
            ["TraumatizedByEvil"] = "Seasonal Halloween effect. Only works when the game build/event supports it.",
            ["WhiteCandy"] = "Seasonal Halloween effect. Only works when the game build/event supports it.",
        };
    }
}

namespace CandyCustomizer
{
    using System;
    using Exiled.API.Features;
    using HarmonyLib;
    using CandyCustomizer.Configuration;
    using CandyCustomizer.Patches;
    using CandyCustomizer.Runtime;
    using ExiledPlugin = Exiled.API.Features.Plugin<CandyCustomizer.Configuration.Config>;
    using PlayerEvents = Exiled.Events.Handlers.Player;
    using Scp330Events = Exiled.Events.Handlers.Scp330;

    public sealed class Plugin : ExiledPlugin
    {
        private const string HarmonyIdPrefix = "candycustomizer";

        private Harmony harmony;
        private CandyEventHandler eventHandler;

        public static Plugin Instance { get; private set; }

        public override string Author => "Candy Customizer";

        public override string Name => "Candy Customizer";

        public override string Prefix => "candy_customizer";

        public override Version Version => new Version(1, 0, 0);

        public override Version RequiredExiledVersion => new Version(9, 14, 0);

        public override void OnEnabled()
        {
            Instance = this;
            OutcomeRepository.LoadOrCreate(this);
            LogValidationWarnings(ConfigValidator.Validate(Config, OutcomeRepository.Current));
            eventHandler = new CandyEventHandler(this);

            harmony = new Harmony($"{HarmonyIdPrefix}.{DateTime.UtcNow.Ticks}");

            try
            {
                CandyEffectPatch.Patch(harmony);
                CandySpawnPoolPatch.Patch(harmony);
            }
            catch
            {
                Cleanup();
                throw;
            }

            if (CandyEffectPatch.PatchedMethodCount == 0)
                Log.Warn("No SCP-330 candy effect methods were patched. Additive mode will still work, but OverrideVanilla may not skip vanilla effects.");

            if (Config.Debug)
            {
                Log.Debug($"Patched {CandyEffectPatch.PatchedMethodCount} SCP-330 candy effect methods.");
                Log.Debug($"Candy spawn pool patch enabled: {CandySpawnPoolPatch.IsPatched}.");
            }

            Scp330Events.EatingScp330 += eventHandler.OnEatingScp330;
            Scp330Events.EatenScp330 += eventHandler.OnEatenScp330;
            PlayerEvents.Dying += eventHandler.OnDying;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Cleanup();
            base.OnDisabled();
        }

        internal void LogValidationWarnings(ConfigValidationResult validation)
        {
            if (validation is null || !validation.HasWarnings)
                return;

            foreach (string warning in validation.Warnings)
                Log.Warn($"Candy Customizer config warning: {warning}");
        }

        private void Cleanup()
        {
            if (eventHandler is not null)
            {
                Scp330Events.EatingScp330 -= eventHandler.OnEatingScp330;
                Scp330Events.EatenScp330 -= eventHandler.OnEatenScp330;
                PlayerEvents.Dying -= eventHandler.OnDying;
            }

            harmony?.UnpatchAll(harmony.Id);
            harmony = null;
            eventHandler = null;

            PendingCandyEffects.Clear();
            PendingCandyDeathReasons.Clear();
            Instance = null;
        }
    }
}

namespace CandyCustomizer.Runtime
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Scp330;
    using CandyCustomizer.Configuration;

    internal sealed class CandyEventHandler
    {
        private readonly Plugin plugin;

        public CandyEventHandler(Plugin plugin)
        {
            this.plugin = plugin;
        }

        public void OnEatingScp330(EatingScp330EventArgs ev)
        {
            object candy = CandyEventReflection.GetCandy(ev);
            string candyName = CandyNameResolver.Resolve(candy);

            if (!plugin.Config.TryGetCandy(candyName, out CandySettings settings))
            {
                LogCandyTrace($"Eating event ignored: resolved candy '{candyName}' from '{candy?.GetType().FullName ?? "null"}', but no enabled config entry matched.");
                return;
            }

            LogCandyTrace($"Eating event resolved '{candyName}' from '{candy?.GetType().FullName ?? "null"}' with mode {settings.Mode}.");

            if (settings.Mode == CandyApplicationMode.VanillaOnly)
                return;

            if (!settings.CanEat)
            {
                ev.IsAllowed = false;
                return;
            }

            PendingCandyEffects.Register(candy, ev.Player);
        }

        public void OnEatenScp330(EatenScp330EventArgs ev)
        {
            object candy = CandyEventReflection.GetCandy(ev);

            if (PendingCandyEffects.WasHandledByPatch(candy))
            {
                PendingCandyEffects.Forget(candy);
                return;
            }

            if (!PendingCandyEffects.TryTake(candy, out Player player))
                player = ev.Player;

            string candyName = CandyNameResolver.Resolve(candy);

            if (!plugin.Config.TryGetCandy(candyName, out CandySettings settings))
            {
                LogCandyTrace($"Eaten event ignored: resolved candy '{candyName}' from '{candy?.GetType().FullName ?? "null"}', but no enabled config entry matched.");
                return;
            }

            if (settings.Mode == CandyApplicationMode.Additive || settings.Mode == CandyApplicationMode.OverrideVanilla)
            {
                LogCandyTrace($"Applying custom behavior for '{candyName}' after eaten event with mode {settings.Mode}.");
                CandyEffectApplier.Apply(player, settings, plugin.Config.Debug);
            }
        }

        public void OnDying(DyingEventArgs ev)
        {
            PendingCandyDeathReasons.HandleDying(ev);
        }

        private void LogCandyTrace(string message)
        {
            if (plugin.Config.Debug)
                Log.Debug($"Candy Customizer event trace: {message}");
        }
    }
}

namespace CandyCustomizer.Runtime
{
    using System;
    using Exiled.API.Features;
    using CandyCustomizer.Configuration;

    internal static class CandyEffectApplier
    {
        public static void Apply(Player player, CandySettings settings, bool debug)
        {
            if (player is null || settings is null)
                return;

            CandyBehaviorSettings behavior = CandyOutcomeSelector.Resolve(settings, player, debug);

            if (behavior is null)
            {
                if (debug)
                    Log.Debug($"No eligible candy outcome was selected for {player.Nickname}.");

                return;
            }

            PendingCandyDeathReasons.TrackEffectDeaths(player, behavior);
            ApplyEffects(player, behavior, debug);
            ApplyHealth(player, behavior);
            ApplyArtificialHealth(player, behavior);

            if (behavior.Explode)
            {
                PendingCandyDeathReasons.TrackExplosionDeath(player, behavior.KillReason);
                PlayerActions.Explode(player);
            }

            if (behavior.Kill)
                PlayerActions.Kill(player, behavior.KillReason);

            if (!string.IsNullOrWhiteSpace(behavior.Hint))
                PlayerActions.ShowHint(player, behavior.Hint, behavior.HintDuration);
        }

        private static void ApplyEffects(Player player, CandyBehaviorSettings settings, bool debug)
        {
            if (settings.Effects is null)
                return;

            foreach (EffectSettings effect in settings.Effects)
            {
                if (effect is null || string.IsNullOrWhiteSpace(effect.Name))
                    continue;

                try
                {
                    PlayerActions.EnableEffect(player, effect.Name, effect.Intensity, effect.Duration, effect.AddDurationIfActive);
                }
                catch (Exception exception)
                {
                    if (debug)
                        Log.Debug($"Failed to apply candy effect '{effect.Name}' to {player.Nickname}: {exception}");
                }
            }
        }

        private static void ApplyHealth(Player player, CandyBehaviorSettings settings)
        {
            if (settings.MaxHealth >= 0f)
                player.MaxHealth = settings.MaxHealth;

            if (Math.Abs(settings.Health) <= 0.001f)
                return;

            if (settings.Health > 0f)
            {
                float targetHealth = player.Health + settings.Health;
                player.Health = Math.Min(targetHealth, player.MaxHealth);
                return;
            }

            PlayerActions.Hurt(player, Math.Abs(settings.Health), GetDamageReason(settings));
        }

        private static void ApplyArtificialHealth(Player player, CandyBehaviorSettings settings)
        {
            if (settings.MaxArtificialHealth >= 0f)
                player.MaxArtificialHealth = settings.MaxArtificialHealth;

            if (Math.Abs(settings.ArtificialHealth) <= 0.001f)
                return;

            player.ArtificialHealth = Math.Max(0f, player.ArtificialHealth + settings.ArtificialHealth);
        }

        private static string GetDamageReason(CandyBehaviorSettings settings)
        {
            return string.IsNullOrWhiteSpace(settings.KillReason) ? "SCP-330 candy" : settings.KillReason;
        }
    }
}

namespace CandyCustomizer.Runtime
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;
    using CandyCustomizer.Configuration;
    using CandyCustomizer.Metadata;

    internal static class PendingCandyDeathReasons
    {
        private static readonly TimeSpan ExplosionWindow = TimeSpan.FromSeconds(3);

        private static readonly Dictionary<Player, PendingDeathReason> PendingReasons = new Dictionary<Player, PendingDeathReason>();
        private static readonly HashSet<Player> ReplayedDeaths = new HashSet<Player>();

        public static void TrackEffectDeaths(Player player, CandyBehaviorSettings settings)
        {
            if (player is null || settings?.Effects is null || string.IsNullOrWhiteSpace(settings.KillReason))
                return;

            HashSet<DamageType> damageTypes = new HashSet<DamageType>();
            DateTime expiresAt = DateTime.UtcNow;

            foreach (EffectSettings effect in settings.Effects)
            {
                if (effect is null || string.IsNullOrWhiteSpace(effect.Name) || !CandyMetadataRegistry.TryGetLethalEffectDamageType(effect.Name, out DamageType damageType))
                    continue;

                damageTypes.Add(damageType);
                expiresAt = GetLaterExpiry(expiresAt, effect.Duration);
            }

            if (damageTypes.Count == 0)
                return;

            Track(player, settings.KillReason, expiresAt, damageTypes);
        }

        public static void TrackExplosionDeath(Player player, string reason)
        {
            if (player is null || string.IsNullOrWhiteSpace(reason))
                return;

            Track(
                player,
                reason,
                DateTime.UtcNow.Add(ExplosionWindow),
                new HashSet<DamageType> { DamageType.Explosion });
        }

        public static void HandleDying(DyingEventArgs ev)
        {
            if (ev?.Player is null)
                return;

            if (ReplayedDeaths.Remove(ev.Player))
                return;

            if (!PendingReasons.TryGetValue(ev.Player, out PendingDeathReason pending))
                return;

            if (DateTime.UtcNow > pending.ExpiresAt)
            {
                PendingReasons.Remove(ev.Player);
                return;
            }

            if (!pending.DamageTypes.Contains(ev.DamageHandler.Type))
                return;

            PendingReasons.Remove(ev.Player);
            ev.IsAllowed = false;
            ReplayedDeaths.Add(ev.Player);
            PlayerActions.Kill(ev.Player, pending.Reason);
        }

        public static void Clear()
        {
            PendingReasons.Clear();
            ReplayedDeaths.Clear();
        }

        private static void Track(Player player, string reason, DateTime expiresAt, HashSet<DamageType> damageTypes)
        {
            if (!PendingReasons.TryGetValue(player, out PendingDeathReason pending))
            {
                PendingReasons[player] = new PendingDeathReason(reason, expiresAt, damageTypes);
                return;
            }

            pending.Reason = reason;
            pending.ExpiresAt = pending.ExpiresAt > expiresAt ? pending.ExpiresAt : expiresAt;

            foreach (DamageType damageType in damageTypes)
                pending.DamageTypes.Add(damageType);
        }

        private static DateTime GetLaterExpiry(DateTime current, float duration)
        {
            if (duration <= 0f)
                return DateTime.MaxValue;

            DateTime expiry = DateTime.UtcNow.AddSeconds(duration);
            return current > expiry ? current : expiry;
        }

        private sealed class PendingDeathReason
        {
            public PendingDeathReason(string reason, DateTime expiresAt, HashSet<DamageType> damageTypes)
            {
                Reason = reason;
                ExpiresAt = expiresAt;
                DamageTypes = damageTypes;
            }

            public string Reason { get; set; }

            public DateTime ExpiresAt { get; set; }

            public HashSet<DamageType> DamageTypes { get; }
        }
    }
}

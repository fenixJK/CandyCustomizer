namespace CandyCustomizer.Runtime
{
    using System.Collections.Generic;
    using Exiled.API.Features;

    internal static class PendingCandyEffects
    {
        private static readonly Dictionary<object, Player> PendingPlayers = new Dictionary<object, Player>(ReferenceComparer.Instance);
        private static readonly HashSet<object> PatchHandledCandies = new HashSet<object>(ReferenceComparer.Instance);

        public static void Register(object candy, Player player)
        {
            if (candy is null || player is null)
                return;

            PendingPlayers[candy] = player;
        }

        public static bool TryGetPlayer(object candy, out Player player)
        {
            player = null;
            return candy is not null && PendingPlayers.TryGetValue(candy, out player);
        }

        public static bool TryTake(object candy, out Player player)
        {
            if (!TryGetPlayer(candy, out player))
                return false;

            PendingPlayers.Remove(candy);
            return true;
        }

        public static void MarkHandledByPatch(object candy)
        {
            if (candy is not null)
                PatchHandledCandies.Add(candy);
        }

        public static bool WasHandledByPatch(object candy)
        {
            if (candy is null)
                return false;

            bool wasHandled = PatchHandledCandies.Contains(candy);
            PatchHandledCandies.Remove(candy);
            return wasHandled;
        }

        public static void Forget(object candy)
        {
            if (candy is null)
                return;

            PendingPlayers.Remove(candy);
            PatchHandledCandies.Remove(candy);
        }

        public static void Clear()
        {
            PendingPlayers.Clear();
            PatchHandledCandies.Clear();
        }
    }
}

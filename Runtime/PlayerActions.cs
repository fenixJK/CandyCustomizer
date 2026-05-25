namespace CandyCustomizer.Runtime
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Exiled.API.Enums;
    using Exiled.API.Features;

    internal static class PlayerActions
    {
        private static readonly MethodInfo ShowHintMethod = FindMethod(
            nameof(Player.ShowHint),
            typeof(string),
            typeof(float));

        private static readonly MethodInfo HurtWithReasonMethod = FindMethod(
            nameof(Player.Hurt),
            typeof(float),
            typeof(string),
            typeof(string));

        private static readonly MethodInfo KillWithReasonMethod = FindMethod(
            nameof(Player.Kill),
            typeof(string),
            typeof(string));

        private static readonly MethodInfo ExplodeMethod = FindMethod(nameof(Player.Explode));

        private static readonly MethodInfo EnableEffectMethod = typeof(Player)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(method =>
            {
                if (method.Name != nameof(Player.EnableEffect))
                    return false;

                ParameterInfo[] parameters = method.GetParameters();
                return parameters.Length == 4
                       && parameters[0].ParameterType == typeof(EffectType)
                       && parameters[1].ParameterType == typeof(byte)
                       && parameters[2].ParameterType == typeof(float)
                       && parameters[3].ParameterType == typeof(bool);
            });

        public static void ShowHint(Player player, string message, float duration)
        {
            ShowHintMethod?.Invoke(player, new object[] { message, duration });
        }

        public static void EnableEffect(Player player, string effectName, byte intensity, float duration, bool addDurationIfActive)
        {
            if (EnableEffectMethod is null || !Enum.TryParse(effectName, true, out EffectType effectType))
                return;

            EnableEffectMethod.Invoke(player, new object[] { effectType, intensity, duration, addDurationIfActive });
        }

        public static void Hurt(Player player, float damage, string reason)
        {
            HurtWithReasonMethod?.Invoke(player, new object[] { damage, reason, string.Empty });
        }

        public static void Kill(Player player, string reason)
        {
            KillWithReasonMethod?.Invoke(player, new object[] { reason, string.Empty });
        }

        public static void Explode(Player player)
        {
            ExplodeMethod?.Invoke(player, null);
        }

        private static MethodInfo FindMethod(string methodName, params Type[] parameterTypes)
        {
            return typeof(Player).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public, null, parameterTypes, null);
        }
    }
}

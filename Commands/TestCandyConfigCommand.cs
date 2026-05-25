namespace CandyCustomizer.Commands
{
    using System;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using CandyCustomizer.Configuration;
    using CandyCustomizer.Runtime;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class TestCandyConfigCommand : ICommand
    {
        public string Command => "candytest";

        public string[] Aliases => new[] { "cctest", "testcandy" };

        public string Description => "Applies the configured custom Candy Customizer behavior to a player for testing.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("candycustomizer.test"))
            {
                response = "You do not have the candycustomizer.test permission.";
                return false;
            }

            string candyName = GetArgument(arguments, 0);

            if (!CandyCommandSupport.TryGetCandy(candyName, out CandySettings settings, out response))
                return false;

            if (!TryGetTarget(arguments, sender, out Player target, out response))
                return false;

            if (!settings.CanEat)
            {
                response = $"Candy '{candyName}' is configured with can_eat: false, so no test behavior was applied.";
                return false;
            }

            if (settings.Mode == CandyApplicationMode.VanillaOnly)
            {
                response = $"Candy '{candyName}' is VanillaOnly, so it has no custom behavior to test.";
                return false;
            }

            CandyEffectApplier.Apply(target, settings, Plugin.Instance.Config.Debug);
            response = $"Applied Candy Customizer's configured custom '{candyName}' behavior to {target.Nickname}. Vanilla SCP:SL candy behavior is not simulated by this command.";
            return true;
        }

        private static bool TryGetTarget(ArraySegment<string> arguments, ICommandSender sender, out Player target, out string response)
        {
            string targetArgument = GetArgument(arguments, 1);

            if (!string.IsNullOrWhiteSpace(targetArgument))
            {
                if (Player.TryGet(targetArgument, out target))
                {
                    response = null;
                    return true;
                }

                response = $"Target player '{targetArgument}' was not found.";
                return false;
            }

            if (Player.TryGet(sender, out target))
            {
                response = null;
                return true;
            }

            response = "A target player is required when running candytest from the server console.";
            return false;
        }

        private static string GetArgument(ArraySegment<string> arguments, int index)
        {
            return index >= 0 && index < arguments.Count ? arguments.Array[arguments.Offset + index] : null;
        }
    }
}

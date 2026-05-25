namespace CandyCustomizer.Commands
{
    using System;
    using CommandSystem;
    using Exiled.Permissions.Extensions;
    using CandyCustomizer.Configuration;
    using CandyCustomizer.Runtime;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class InspectCandyConfigCommand : ICommand
    {
        public string Command => "candyinspect";

        public string[] Aliases => new[] { "ccinspect", "inspectcandy" };

        public string Description => "Shows the configured Candy Customizer behavior for one candy.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("candycustomizer.inspect"))
            {
                response = "You do not have the candycustomizer.inspect permission.";
                return false;
            }

            string candyName = GetArgument(arguments, 0);

            if (!CandyCommandSupport.TryGetCandy(candyName, out CandySettings settings, out response))
                return false;

            response = CandySettingsFormatter.Format(candyName, settings);
            return true;
        }

        private static string GetArgument(ArraySegment<string> arguments, int index)
        {
            return index >= 0 && index < arguments.Count ? arguments.Array[arguments.Offset + index] : null;
        }
    }
}

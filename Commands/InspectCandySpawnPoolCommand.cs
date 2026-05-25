namespace CandyCustomizer.Commands
{
    using System;
    using CommandSystem;
    using Exiled.Permissions.Extensions;
    using CandyCustomizer.Runtime;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class InspectCandySpawnPoolCommand : ICommand
    {
        public string Command => "candypool";

        public string[] Aliases => new[] { "candyweights", "ccpool" };

        public string Description => "Shows Candy Customizer's effective random SCP-330 spawn pool and resolved weights.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("candycustomizer.inspect"))
            {
                response = "You do not have the candycustomizer.inspect permission.";
                return false;
            }

            return CandySpawnPoolSelector.TryDescribePool(out response);
        }
    }
}

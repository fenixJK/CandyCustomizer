namespace CandyCustomizer.Commands
{
    using System;
    using CommandSystem;
    using Exiled.Permissions.Extensions;
    using CandyCustomizer.Configuration;
    using CandyCustomizer.Runtime;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class InspectOutcomeConfigCommand : ICommand
    {
        public string Command => "outcomeinspect";

        public string[] Aliases => new[] { "ccoutcome", "inspectoutcome" };

        public string Description => "Shows one reusable Candy Customizer outcome payload from outcomes.yml.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("candycustomizer.inspect"))
            {
                response = "You do not have the candycustomizer.inspect permission.";
                return false;
            }

            string outcomeName = GetArgument(arguments, 0);

            if (string.IsNullOrWhiteSpace(outcomeName))
            {
                response = "An outcome name is required.";
                return false;
            }

            if (!OutcomeRepository.TryGet(outcomeName, out CandyOutcomeSettings outcome))
            {
                response = $"Outcome '{outcomeName}' was not found in outcomes.yml.";
                return false;
            }

            response = CandySettingsFormatter.FormatOutcome(outcomeName, outcome);
            return true;
        }

        private static string GetArgument(ArraySegment<string> arguments, int index)
        {
            return index >= 0 && index < arguments.Count ? arguments.Array[arguments.Offset + index] : null;
        }
    }
}

namespace CandyCustomizer.Commands
{
    using System;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.API.Interfaces;
    using Exiled.Loader;
    using Exiled.Permissions.Extensions;
    using CandyCustomizer.Configuration;
    using CandyCustomizer.Runtime;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class ReloadCandyConfigCommand : ICommand
    {
        public string Command => "candyreload";

        public string[] Aliases => new[] { "ccreload", "reloadcandy" };

        public string Description => "Reloads only the Candy Customizer config.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("candycustomizer.reload"))
            {
                response = "You do not have the candycustomizer.reload permission.";
                return false;
            }

            if (Plugin.Instance is null)
            {
                response = "Candy Customizer plugin instance was not found.";
                return false;
            }

            IPlugin<IConfig> plugin = Loader.GetPlugin(Plugin.Instance.Prefix);

            if (plugin is null)
            {
                response = "Candy Customizer plugin instance was not found.";
                return false;
            }

            try
            {
                plugin.LoadConfig();
                ManifestFileWriter.Write(Plugin.Instance);
                OutcomeRepository.LoadOrCreate(Plugin.Instance);
                ConfigValidationResult validation = ConfigValidator.Validate(Plugin.Instance.Config, OutcomeRepository.Current);
                Plugin.Instance.LogValidationWarnings(validation);

                if (plugin.Config.Debug)
                    Log.DebugEnabled.Add(plugin.Assembly);
                else
                    Log.DebugEnabled.Remove(plugin.Assembly);

                response = validation.HasWarnings
                    ? $"Candy Customizer config reloaded with {validation.Warnings.Count} warning(s). Check the server console."
                    : "Candy Customizer config reloaded. Saved YAML changes are now active.";
                return true;
            }
            catch (Exception exception)
            {
                Log.Error($"Candy Customizer config reload failed: {exception}");
                response = "Candy Customizer config reload failed. Check the server console for details.";
                return false;
            }
        }
    }
}

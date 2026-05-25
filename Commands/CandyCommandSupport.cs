namespace CandyCustomizer.Commands
{
    using CandyCustomizer.Configuration;

    internal static class CandyCommandSupport
    {
        public static bool TryGetCandy(string candyName, out CandySettings settings, out string response)
        {
            settings = null;

            if (Plugin.Instance is null)
            {
                response = "Candy Customizer plugin instance was not found.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(candyName))
            {
                response = "A candy name is required.";
                return false;
            }

            if (!Plugin.Instance.Config.TryGetCandy(candyName, out settings))
            {
                response = $"Candy '{candyName}' is not enabled or does not exist in the current config.";
                return false;
            }

            response = null;
            return true;
        }
    }
}

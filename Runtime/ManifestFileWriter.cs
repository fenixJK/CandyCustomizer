namespace CandyCustomizer.Runtime
{
    using System;
    using System.IO;
    using Exiled.API.Features;
    using CandyCustomizer.Configuration;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;

    internal static class ManifestFileWriter
    {
        private const string ManifestFileName = "manifest.yml";

        private static readonly ISerializer Serializer = new SerializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreFields()
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitDefaults)
            .Build();

        public static string GetManifestPath(Plugin plugin)
        {
            return Path.Combine(Path.GetDirectoryName(plugin.ConfigPath), ManifestFileName);
        }

        public static void Write(Plugin plugin)
        {
            try
            {
                string directory = Path.GetDirectoryName(plugin.ConfigPath);

                if (string.IsNullOrWhiteSpace(directory))
                    return;

                Directory.CreateDirectory(directory);

                string manifestPath = GetManifestPath(plugin);
                File.WriteAllText(manifestPath, Serializer.Serialize(new ConfigurationManifest()));

                if (plugin.Config.Debug)
                    Log.Debug($"Candy Customizer manifest file written to {manifestPath}.");
            }
            catch (Exception exception)
            {
                Log.Warn($"Candy Customizer could not write manifest file: {exception}");
            }
        }
    }
}

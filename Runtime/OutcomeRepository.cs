namespace CandyCustomizer.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using CandyCustomizer.Configuration;
    using Exiled.API.Features;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;

    internal static class OutcomeRepository
    {
        private const string OutcomeFileName = "outcomes.yml";

        private static readonly ISerializer Serializer = new SerializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreFields()
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
            .Build();

        private static readonly IDeserializer Deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        public static OutcomeConfiguration Current { get; private set; } = OutcomeDefaults.Create();

        public static string GetPath(Plugin plugin)
        {
            return Path.Combine(Path.GetDirectoryName(plugin.ConfigPath), OutcomeFileName);
        }

        public static void LoadOrCreate(Plugin plugin)
        {
            string path = GetPath(plugin);

            try
            {
                string directory = Path.GetDirectoryName(path);

                if (!string.IsNullOrWhiteSpace(directory))
                    Directory.CreateDirectory(directory);

                if (!File.Exists(path))
                    File.WriteAllText(path, Serializer.Serialize(OutcomeDefaults.Create()));

                string yaml = File.ReadAllText(path);
                OutcomeConfiguration configuration = Deserializer.Deserialize<OutcomeConfiguration>(yaml) ?? new OutcomeConfiguration();
                Current = configuration;

                if (plugin.Config.Debug)
                    Log.Debug($"Candy Customizer outcomes loaded from {path}.");
            }
            catch (Exception exception)
            {
                Current = new OutcomeConfiguration();
                Log.Warn($"Candy Customizer could not load outcomes from {path}: {exception}");
            }
        }

        public static bool TryGet(string name, out CandyOutcomeSettings outcome)
        {
            outcome = null;

            if (string.IsNullOrWhiteSpace(name) || Current?.Outcomes is null)
                return false;

            foreach (KeyValuePair<string, CandyOutcomeSettings> pair in Current.Outcomes)
            {
                if (string.Equals(pair.Key, name, StringComparison.OrdinalIgnoreCase))
                {
                    outcome = pair.Value;
                    return outcome is not null;
                }
            }

            return false;
        }
    }
}

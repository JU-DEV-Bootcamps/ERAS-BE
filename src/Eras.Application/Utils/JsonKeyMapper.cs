using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Eras.Application.Utils
{
    public static class JsonKeyMapper
    {
        private static readonly Dictionary<string, string> _keyMappings;

        static JsonKeyMapper()
        {
            var jsonFilePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "src",
                "Eras.Application",
                "Resources",
                "JsonMappings.json"
            );
            if (File.Exists(jsonFilePath))
            {
                var json = File.ReadAllText(jsonFilePath);
                _keyMappings = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
            }
            else
            {
                _keyMappings = new Dictionary<string, string>();
            }
        }

        public static string GetJsonKey(string propertyName)
        {
            return _keyMappings.TryGetValue(propertyName, out var jsonKey) ? jsonKey : propertyName;
        }
    }
}

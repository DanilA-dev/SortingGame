using System.Collections.Generic;
using D_Dev.SaveSystem.Converters;
using Newtonsoft.Json;

namespace D_Dev.SaveSystem
{
    public static class SaveSerializer
    {
        #region Fields

        public static readonly JsonSerializerSettings Settings = new()
        {
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Converters = new List<JsonConverter>
            {
                new Vector2Converter(),
                new Vector3Converter(),
                new QuaternionConverter(),
                new ColorConverter()
            }
        };

        public static readonly JsonSerializer Serializer = JsonSerializer.Create(Settings);

        #endregion
    }
}

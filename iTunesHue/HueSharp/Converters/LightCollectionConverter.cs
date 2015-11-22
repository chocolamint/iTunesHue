using System;
using HueSharp.Lights;
using Newtonsoft.Json;

namespace HueSharp.Converters
{
    /// <summary>
    /// Convert list of lights.
    /// </summary>
    internal class LightCollectionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(LightCollection);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var collection = new LightCollection();
            while (true)
            {
                // read json key.
                reader.Read();
                var id = (string)reader.Value;
                // null is the end of json.
                if (id == null) break;

                // read json value.
                reader.Read();
                var light = serializer.Deserialize<Light>(reader);

                // set light identifier.
                light.Id = id;

                collection.Add(light);
            }
            return collection;
        }
    }
}
using System;
using HueSharp.Lights;
using Newtonsoft.Json;

namespace HueSharp.Converters
{
    /// <summary>
    /// Converter of xy json e.g. [0.5128,0.4147].
    /// </summary>
    internal class XyConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Xy);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Read [0.5128,0.4147] -> new Xy(0.5128, 0.4147)
            var x = (float)reader.ReadAsDecimal();
            var y = (float)reader.ReadAsDecimal();
            // Skip Array end.
            reader.Read();

            return new Xy(x, y);
        }
    }
}
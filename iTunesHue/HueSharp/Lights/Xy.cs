using System.Runtime.Serialization;
using HueSharp.Converters;
using Newtonsoft.Json;

namespace HueSharp.Lights
{
    [DataContract]
    [JsonConverter(typeof(XyConverter))]
    public struct Xy
    {
        public float X { get; }
        public float Y { get; }

        public Xy(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}
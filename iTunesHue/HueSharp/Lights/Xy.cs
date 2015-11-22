using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;
using HueSharp.Converters;
using Newtonsoft.Json;
using static System.Math;

namespace HueSharp.Lights
{
    [DataContract]
    [JsonConverter(typeof(XyConverter))]
    [DebuggerDisplay(@"[{X}, {Y}]")]
    public struct Xy
    {
        public float X { get; }
        public float Y { get; }

        public Xy(float x, float y)
        {
            X = (float)(Floor(x * 10000) / 10000);
            Y = (float)(Floor(y * 10000) / 10000);
        }

        public static Xy FromColor(Color color)
        {
            return FromRgb(color.R, color.G, color.B);
        }
        public static Xy FromRgb(byte r, byte g, byte b)
        {
            var red = r / 255.0f;
            var green = g / 255.0f;
            var blue = b / 255.0f;

            red = (red > 0.04045f) ? (float)Pow((red + 0.055f) / (1.0f + 0.055f), 2.4f) : (red / 12.92f);
            green = (green > 0.04045f) ? (float)Pow((green + 0.055f) / (1.0f + 0.055f), 2.4f) : (green / 12.92f);
            blue = (blue > 0.04045f) ? (float)Pow((blue + 0.055f) / (1.0f + 0.055f), 2.4f) : (blue / 12.92f);

            var tempX = red * 0.664511f + green * 0.154324f + blue * 0.162028f;
            var tempY = red * 0.283881f + green * 0.668433f + blue * 0.047685f;
            var tempZ = red * 0.000088f + green * 0.072310f + blue * 0.986039f;

            var x = tempX / (tempX + tempY + tempZ);
            var y = tempY / (tempX + tempY + tempZ);

            return new Xy(x, y);
        }
    }
}
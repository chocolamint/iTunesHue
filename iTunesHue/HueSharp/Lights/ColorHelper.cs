using System;
using System.Drawing;
using static System.Math;

namespace HueSharp.Lights
{
    public static class ColorHelper
    {
        public static Color GetColor(this LightState state)
        {
            return XyToColor(state.Xy.X, state.Xy.Y, state.Brightness);
        }
        public static Color XyToColor(float x, float y, float brightness)
        {
            var z = 1.0f - x - y;
            var tempY = brightness; // The given brightness value
            var tempX = (tempY / y) * x;
            var tempZ = (tempY / y) * z;

            var r = tempX * 1.656492f - tempY * 0.354851f - tempZ * 0.255038f;
            var g = -tempX * 0.707196f + tempY * 1.655397f + tempZ * 0.036152f;
            var b = tempX * 0.051713f - tempY * 0.121364f + tempZ * 1.011530f;

            if (r > b && r > g && r > 1.0f)
            {
                // red is too big
                g = g / r;
                b = b / r;
                r = 1.0f;
            }
            else if (g > b && g > r && g > 1.0f)
            {
                // green is too big
                r = r / g;
                b = b / g;
                g = 1.0f;
            }
            else if (b > r && b > g && b > 1.0f)
            {
                // blue is too big
                r = r / b;
                g = g / b;
                b = 1.0f;
            }

            r = r <= 0.0031308f ? 12.92f * r : (1.0f + 0.055f) * (float)Pow(r, 1.0f / 2.4f) - 0.055f;
            g = g <= 0.0031308f ? 12.92f * g : (1.0f + 0.055f) * (float)Pow(g, 1.0f / 2.4f) - 0.055f;
            b = b <= 0.0031308f ? 12.92f * b : (1.0f + 0.055f) * (float)Pow(b, 1.0f / 2.4f) - 0.055f;

            if (r > b && r > g)
            {
                // red is biggest
                if (r > 1.0f)
                {
                    g = g / r;
                    b = b / r;
                    r = 1.0f;
                }
            }
            else if (g > b && g > r)
            {
                // green is biggest
                if (g > 1.0f)
                {
                    r = r / g;
                    b = b / g;
                    g = 1.0f;
                }
            }
            else if (b > r && b > g)
            {
                // blue is biggest
                if (b > 1.0f)
                {
                    r = r / b;
                    g = g / b;
                    b = 1.0f;
                }
            }

            var red = (byte)Round(r * byte.MaxValue, MidpointRounding.AwayFromZero);
            var green = (byte)Round(g * byte.MaxValue, MidpointRounding.AwayFromZero);
            var blue = (byte)Round(b * byte.MaxValue, MidpointRounding.AwayFromZero);

            return Color.FromArgb(red, green, blue);
        }
    }
}

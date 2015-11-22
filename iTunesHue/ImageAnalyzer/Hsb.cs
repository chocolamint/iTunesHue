using System;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace ImageAnalyzer
{
    /// <summary>
    /// <see cref="Hue"/>、<see cref="Saturation"/>、<see cref="Brightness"/> の各情報を保持します。
    /// </summary>
    [DebuggerDisplay("hsb({Hue}, {Saturation*100}%, {Brightness*100}%)")]
    public struct Hsb
    {
        public float Hue { get; }
        public float Saturation { get; }
        public float Brightness { get; }

        public Hsb(float hue, float saturation, float brightness)
        {
            Hue = hue;
            Saturation = saturation;
            Brightness = brightness;
        }
        public static Hsb FromColor(Color color)
        {
            return new Hsb(GetHue(color), GetSaturation(color), GetBrightness(color));
        }

        public static float GetHue(Color color)
        {
            var max = Math.Max(color.R, Math.Max(color.G, color.B));
            var min = Math.Min(color.R, Math.Min(color.G, color.B));
            var c = max - min;

            if (c == 0) return 0;
            float hue;
            if (max == color.R)
            {
                hue = (color.G - color.B) / (float)c;
            }
            else if (max == color.G)
            {
                hue = (color.B - color.R) / (float)c + 2f;
            }
            else
            {
                hue = (color.R - color.G) / (float)c + 4f;
            }
            hue *= 60f;
            if (hue < 0f)
            {
                hue += 360f;
            }
            return hue;
        }

        public static float GetSaturation(Color color)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            return (max == 0) ? 0 : 1f - (min / (float)max);
        }

        public static float GetBrightness(Color color)
        {
            return Math.Max(color.R, Math.Max(color.G, color.B)) / (float)byte.MaxValue;
        }

        public static float GetLuminance(Color color)
        {
            return (float)(color.R * 0.298912 + color.G * 0.586611 + color.B * 0.114478);
        }

        public Hsb AddBrightness(float brightness)
        {
            return new Hsb(Hue, Saturation, Brightness - brightness);
        }

        public Hsb SetBrightness(float brightness)
        {
            return new Hsb(Hue, Saturation, brightness);
        }

        public Color ToColor()
        {
            var v = Brightness;
            var s = Saturation;

            float r, g, b;
            if (Math.Abs(s) < float.Epsilon)
            {
                r = v;
                g = v;
                b = v;
            }
            else
            {
                var h = Hue / 60f;
                var i = (int)Math.Floor(h);
                var f = h - i;
                var p = v * (1f - s);
                float q;
                if (i % 2 == 0)
                {
                    //t
                    q = v * (1f - (1f - f) * s);
                }
                else
                {
                    q = v * (1f - f * s);
                }

                switch (i)
                {
                    case 0:
                        r = v;
                        g = q;
                        b = p;
                        break;
                    case 1:
                        r = q;
                        g = v;
                        b = p;
                        break;
                    case 2:
                        r = p;
                        g = v;
                        b = q;
                        break;
                    case 3:
                        r = p;
                        g = q;
                        b = v;
                        break;
                    case 4:
                        r = q;
                        g = p;
                        b = v;
                        break;
                    case 5:
                        r = v;
                        g = p;
                        b = q;
                        break;
                    default:
                        throw new ArgumentException();
                }
            }

            return Color.FromArgb(
                (int)Math.Round(r * 255f),
                (int)Math.Round(g * 255f),
                (int)Math.Round(b * 255f));
        }
    }
}

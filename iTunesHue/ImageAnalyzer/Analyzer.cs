using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ImageAnalyzer
{
    public static class Analyzer
    {
        public static Tuple<Color, Color, Color> GetColors(Bitmap bitmap)
        {
            var baseColor = FindMainColor(bitmap);
            var accentColor = FindAccentColor(bitmap, baseColor);
            var subColorX = FindSubColor(bitmap, baseColor, accentColor);

            return Tuple.Create(baseColor, accentColor, subColorX);
        }
        private static Color FindMainColor(Bitmap bitmap)
        {
            return FindEdgeColor(bitmap);
        }
        private static Color FindAccentColor(Bitmap bitmap, Color baseColor)
        {
            return FindBackgroundColor(bitmap, x => !x.IsNearly(baseColor));
        }
        private static Color FindSubColor(Bitmap bitmap, Color mainColor, Color accentColor)
        {
            var excepts = new HashSet<Color>(new[] { mainColor, accentColor }, new ColorNearlyComparer());
            var subColor = FindBackgroundColor(bitmap, x => !excepts.Contains(x));
            if (subColor.IsEmpty)
            {
                return mainColor;
            }
            var allPixels = bitmap.GetPixels().ToArray();
            if (allPixels.Count(x => subColor.IsNearly(x)) / (float)allPixels.Count() < 0.05)
            {
                return mainColor;
            }
            return subColor;
        }
        private static Color FindEdgeColor(Bitmap bitmap, Func<Color, bool> filter = null)
        {
            var colors = from x in Enumerable.Range(0, bitmap.Width)
                         from y in Enumerable.Range(0, bitmap.Height)
                         where x == 0 || x == bitmap.Width || y == 0 || y == bitmap.Height
                         select bitmap.GetPixel(x, y);

            return FindColor(colors, filter);
        }
        private static Color FindBackgroundColor(Bitmap bitmap, Func<Color, bool> filter = null)
        {
            var colors = from x in Enumerable.Range(1, bitmap.Width - 2)
                         from y in Enumerable.Range(1, bitmap.Height - 2)
                         select bitmap.GetPixel(x, y);

            return FindColor(colors, filter);
        }
        private static Color FindColor(IEnumerable<Color> colors, Func<Color, bool> filter = null)
        {
            var array = colors.ToArray();

            var sortedColorCount = (from g in array.GroupBy(x => x)
                                    let color = g.Key
                                    let hsb = Hsb.FromColor(color)
                                    let count = g.Count()
                                    where count > 2
                                    where filter == null || filter(color)
                                    orderby count * Math.Sqrt(hsb.Brightness * hsb.Saturation) descending
                                    select new { color, count }
                ).ToArray();

            var proposedEdgeColor = sortedColorCount.FirstOrDefault();
            if (proposedEdgeColor == null) return Color.Empty;

            if (proposedEdgeColor.color.IsBlackOrWhite())
            {
                var nextProposedEdgeColor = sortedColorCount.Skip(1)
                                                            .FirstOrDefault(x => !x.color.IsBlackOrWhite());
                if (nextProposedEdgeColor != null)
                {
                    return nextProposedEdgeColor.color;
                }
            }
            return proposedEdgeColor.color;
        }

        internal static bool IsBlackOrWhite(this Color color)
        {
            var hsb = Hsb.FromColor(color);

            // Black
            if (hsb.Brightness < 0.18) return true;
            // White
            if (hsb.Saturation < 0.17 && 0.94 < hsb.Brightness) return true;

            return false;
        }

        private static bool IsNearly(this Color self, Color other)
        {
            var xhsb = Hsb.FromColor(self);
            var yhsb = Hsb.FromColor(other);

            var sdiff = Math.Abs(xhsb.Saturation - yhsb.Saturation);
            var bdiff = Math.Abs(xhsb.Brightness - yhsb.Brightness);

            if (sdiff * bdiff > 0.3) return false;

            var weakness = xhsb.Brightness * xhsb.Saturation;
            var threshold = 180 * (1.0f - weakness);
            var hueDistance = GetHueDistance(xhsb, yhsb);

            return hueDistance < threshold;
        }
        private static float GetHueDistance(Hsb left, Hsb right)
        {
            var diff = Math.Abs(left.Hue - right.Hue);
            return (diff < 180f) ? diff : (360f - diff);
        }

        private static IEnumerable<Color> GetPixels(this Bitmap bitmap)
        {
            return from x in Enumerable.Range(0, bitmap.Width)
                   from y in Enumerable.Range(0, bitmap.Height)
                   select bitmap.GetPixel(x, y);
        }
        private class ColorNearlyComparer : IEqualityComparer<Color>
        {
            public bool Equals(Color x, Color y)
            {
                return x.IsNearly(y);
            }

            public int GetHashCode(Color obj)
            {
                return 0;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

namespace ImageAnalyzer
{
    public static class Analyzer
    {
        public static Tuple<Color, Color, Color> GetColors(Bitmap bitmap)
        {
            // TODO: Dispose は呼び出し元がするべき。というか System.Drawing 使わないようにしたい
            try
            {
                var originalWidth = bitmap.Width;
                var originalHeight = bitmap.Height;

                var resizedWidth = 240;
                if (originalWidth > resizedWidth)
                {
                    var resizedHeight = originalHeight * resizedWidth / originalWidth;
                    var resized = new Bitmap(resizedWidth, resizedHeight);

                    using (var g = Graphics.FromImage(resized))
                    {
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.DrawImage(bitmap, 0, 0, resizedWidth, resizedHeight);
                    }

                    bitmap.Dispose();
                    bitmap = resized;
                }

                var baseColor = FindMainColor(bitmap);
                var accentColor = FindAccentColor(bitmap, baseColor);
                var subColorX = FindSubColor(bitmap, baseColor, accentColor);

                return Tuple.Create(baseColor, accentColor, subColorX);
            }
            finally
            {
                bitmap.Dispose();
            }
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

            var allPixels = EnumerateColors(bitmap, (width, height) =>
                from y in Enumerable.Range(0, height)
                from x in Enumerable.Range(0, width)
                select new Point(x, y)).ToArray();

            if (allPixels.Count(x => subColor.IsNearly(x)) / (float)allPixels.Length < 0.05)
            {
                return mainColor;
            }
            return subColor;
        }
        private static Color FindEdgeColor(Bitmap bitmap, Func<Color, bool> filter = null)
        {
            var colors = EnumerateColors(bitmap, (width, height) =>
                from y in Enumerable.Range(0, height)
                from x in Enumerable.Range(0, width)
                where x == 0 || x == bitmap.Width || y == 0 || y == bitmap.Height
                select new Point(x, y));

            return FindColor(colors, filter);
        }
        private static Color FindBackgroundColor(Bitmap bitmap, Func<Color, bool> filter = null)
        {
            var colors = EnumerateColors(bitmap, (width, height) =>
                from y in Enumerable.Range(1, bitmap.Height - 2)
                from x in Enumerable.Range(1, bitmap.Width - 2)
                select new Point(x, y));

            return FindColor(colors, filter);
        }
        private static IEnumerable<Color> EnumerateColors(Bitmap bitmap, Func<int, int, IEnumerable<Point>> pointsSelector)
        {
            // Bitmap.Width と Bitmap.Height の取得は遅いためローカル変数に退避
            var width = bitmap.Width;
            var height = bitmap.Height;
            var rect = new Rectangle(0, 0, width, height);
            var bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);
            try
            {
                var unit = bitmapData.Stride / width;
                var bytes = new byte[bitmapData.Stride * height];
                Marshal.Copy(bitmapData.Scan0, bytes, 0, bytes.Length);

                // 呼び出し元が Bitmap.Width, Bitmap.Height にアクセスしなくて済むように Func<int, int, IEnumerable<Point>> を使う
                var colors = from point in pointsSelector(width, height)
                             let x = point.X
                             let y = point.Y
                             let position = x * unit + bitmapData.Stride * y
                             let b = bytes[position + 0]
                             let g = bytes[position + 1]
                             let r = bytes[position + 2]
                             let color = Color.FromArgb(r, g, b)
                             select color;

                foreach (var color in colors)
                {
                    yield return color;
                }
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }

        /// <summary>
        /// <paramref name="filter"/> を満たす中で最も多く使用されている色を取得します。
        /// </summary>
        /// <param name="colors"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using HueSharp.Lights;

namespace iTunesHue.Utilities
{
    public static class Extensions
    {
        public static Task WhenAll(this IEnumerable<Task> tasks)
        {
            return Task.WhenAll(tasks);
        }

        public static System.Windows.Media.Color ToMediaColor(this Color color)
        {
            return System.Windows.Media.Color.FromRgb(color.R, color.G, color.B);
        }

        public static TDisposable AddTo<TDisposable>(this TDisposable disposable, ICollection<IDisposable> composite)
            where TDisposable : IDisposable
        {
            composite.Add(disposable);
            return disposable;
        }

        public static BitmapImage ToBitmapImage(this Stream stream)
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();
            return image;
        }

        public static Xy ToXy(this Color color)
        {
            return Xy.FromColor(color);
        }
    }
}
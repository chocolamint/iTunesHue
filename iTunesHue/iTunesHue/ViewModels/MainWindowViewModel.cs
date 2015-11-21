using Reactive.Bindings;
using System;
using System.Reactive.Linq;
using System.Windows.Media;
using static System.Windows.Media.Color;
using static System.Math;
using System.Windows;
using HueSharp;
using System.Threading;

namespace iTunesHue.ViewModels
{
    public class MainWindowViewModel
    {
        public ReactiveProperty<byte> R { get; private set; } = new ReactiveProperty<byte>(Byte.MaxValue);
        public ReactiveProperty<byte> G { get; private set; } = new ReactiveProperty<byte>(Byte.MaxValue);
        public ReactiveProperty<byte> B { get; private set; } = new ReactiveProperty<byte>(Byte.MaxValue);

        public ReactiveProperty<Color> Color { get; private set; }

        public ReactiveProperty<double> X { get; private set; }
        public ReactiveProperty<double> Y { get; private set; }

        public MainWindowViewModel()
        {
            Color = Observable.CombineLatest(R, G, B, FromRgb).ToReactiveProperty();

            var xy = Observable.CombineLatest(R, G, B, ToXY).Publish();

            X = xy.Select(p => p.X).ToReactiveProperty();
            Y = xy.Select(p => p.Y).ToReactiveProperty();

            xy.Connect();

            var bridge = new Bridge("192.168.0.15")
            {
                UserName = "265c6606152d58042d87d6524ca2cff"
            };

            //bridge.AuthorizeAsync("iTunesHue", CancellationToken.None).ContinueWith(async t=>
            //{
            var lights = bridge.GetLightsAsync(CancellationToken.None).Result;
            Console.WriteLine(lights);
            //});
        }

        private static Point ToXY(byte r, byte g, byte b)
        {
            var red = (r > 0.04045) ? Pow((r + 0.055) / (1.0f + 0.055), 2.4) : (r / 12.92);
            var green = (g > 0.04045) ? Pow((g + 0.055) / (1.0f + 0.055), 2.4) : (g / 12.92);
            var blue = (b > 0.04045) ? Pow((b + 0.055) / (1.0f + 0.055), 2.4) : (b / 12.92);

            var tempX = red * 0.664511 + green * 0.154324 + blue * 0.162028;
            var tempY = red * 0.283881 + green * 0.668433 + blue * 0.047685;
            var tempZ = red * 0.000088 + green * 0.072310 + blue * 0.986039;

            var x = tempX / (tempX + tempY + tempZ);
            var y = tempY / (tempX + tempY + tempZ);

            return new Point(x, y);
        }
    }
}

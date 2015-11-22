using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Media;
using static System.Windows.Media.Color;
using static System.Math;
using HueSharp;
using System.Threading;
using System.Threading.Tasks;
using HueSharp.Lights;

namespace iTunesHue.ViewModels
{
    public class MainWindowViewModel
    {
        public ReactiveProperty<byte> R { get; } = new ReactiveProperty<byte>(byte.MaxValue);
        public ReactiveProperty<byte> G { get; } = new ReactiveProperty<byte>(byte.MaxValue);
        public ReactiveProperty<byte> B { get; } = new ReactiveProperty<byte>(byte.MaxValue);

        public ReactiveProperty<Color> Color { get; private set; }

        public ReactiveProperty<float> X { get; private set; }
        public ReactiveProperty<float> Y { get; private set; }

        public ReactiveProperty<float> LightX { get; } = new ReactiveProperty<float>();
        public ReactiveProperty<float> LightY { get; } = new ReactiveProperty<float>();

        public ReactiveProperty<byte> LightR { get; } = new ReactiveProperty<byte>();
        public ReactiveProperty<byte> LightG { get; } = new ReactiveProperty<byte>();
        public ReactiveProperty<byte> LightB { get; } = new ReactiveProperty<byte>();

        public ReactiveProperty<Color> LightColor { get; } = new ReactiveProperty<Color>();

        public MainWindowViewModel()
        {
            var bridge = new Bridge("192.168.0.15")
            {
                UserName = "265c6606152d58042d87d6524ca2cff"
            };
            var lights = bridge.GetLightsAsync(CancellationToken.None).Result;

            Color = Observable.CombineLatest(R, G, B, FromRgb).ToReactiveProperty();

            var xy = Observable.CombineLatest(R, G, B, Xy.FromRgb).Publish();

            X = xy.Select(p => p.X).ToReactiveProperty();
            Y = xy.Select(p => p.Y).ToReactiveProperty();

            xy.Throttle(TimeSpan.FromSeconds(1)).Subscribe(async v =>
            {
                await lights.Select(async light =>
                {
                    await light.SetXyAsync(v);

                }).WhenAll();

                var first = lights.First();
                await first.ReloadStateAsync();

                var state = first.State;

                LightX.Value = state.Xy.X;
                LightY.Value = state.Xy.Y;

                var color = state.GetColor();
                var rgb = FromRgb(color.R, color.G, color.B);

                LightR.Value = rgb.R;
                LightG.Value = rgb.G;
                LightB.Value = rgb.B;

                LightColor.Value = rgb;

                R.Value = rgb.R;
                G.Value = rgb.G;
                B.Value = rgb.B;
                Color.Value = rgb;
            });

            xy.Connect();
        }

    }

    public static class TaskEx
    {
        public static Task WhenAll(this IEnumerable<Task> tasks)
        {
            return Task.WhenAll(tasks);
        }
    }
}

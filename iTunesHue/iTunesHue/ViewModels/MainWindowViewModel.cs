using Reactive.Bindings;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Media;
using iTunesSharp;
using System.Windows.Media.Imaging;
using HueSharp;
using HueSharp.Lights;
using iTunesHue.Utilities;
using ImageAnalyzer;
using Livet;
using Color = System.Windows.Media.Color;

namespace iTunesHue.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        private readonly iTunes _iTunes;

        public ReactiveProperty<Color> MainColor { get; } = new ReactiveProperty<Color>();
        public ReactiveProperty<Color> AccentColor { get; } = new ReactiveProperty<Color>();
        public ReactiveProperty<Color> SubColor { get; } = new ReactiveProperty<Color>();

        public ReactiveProperty<ImageSource> Artwork { get; } = new ReactiveProperty<ImageSource>();

        public MainWindowViewModel()
        {
            _iTunes = new iTunes().AddTo(CompositeDisposable);
            
            var bridge = new Bridge("192.168.0.15")
            {
                UserName = "265c6606152d58042d87d6524ca2cff"
            };
            var lights = bridge.GetLightsAsync(CancellationToken.None).Result;

            Observable.Interval(TimeSpan.FromSeconds(1))
                      .Select(_ => _iTunes.GetCurrentTrack())
                      .Where(track => track != null)
                      .DistinctUntilChanged()
                      .Select(track => track.Artwork.FirstOrDefault())
                      .Where(artwork => artwork != null)
                      .SelectMany(artwork => artwork.GetStreamAsync())
                      .Subscribe(async stream =>
                      {
                          var bitmap = new Bitmap(stream);
                          var colors = Analyzer.GetColors(bitmap);

                          MainColor.Value = colors.Item1.ToMediaColor();
                          AccentColor.Value = colors.Item2.ToMediaColor();
                          SubColor.Value = colors.Item3.ToMediaColor();

                          stream.Seek(0, SeekOrigin.Begin);

                          var image = stream.ToBitmapImage();
                          image.Freeze();
                          Artwork.Value = image;

                          await new[]
                          {
                              lights.ElementAt(0).SetXyAsync(colors.Item1.ToXy()),
                              lights.ElementAt(1).SetXyAsync(colors.Item2.ToXy()),
                              lights.ElementAt(2).SetXyAsync(colors.Item1.ToXy()),
                              lights.ElementAt(3).SetXyAsync(colors.Item3.ToXy()),

                          }.WhenAll();
                      })
                      .AddTo(CompositeDisposable);
        }
    }
}

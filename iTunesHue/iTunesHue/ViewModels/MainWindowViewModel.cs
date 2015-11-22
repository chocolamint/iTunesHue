using Reactive.Bindings;
using System;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using iTunesSharp;
using System.Windows.Media.Imaging;
using HueSharp;
using iTunesHue.Utilities;
using ImageAnalyzer;
using Livet;
using DColor = System.Drawing.Color;
using MColor = System.Windows.Media.Color;

namespace iTunesHue.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        private readonly iTunes _iTunes;
        private readonly IObservable<Tuple<DColor, DColor, DColor>> _colors;

        public ReactiveProperty<MColor> MainColor { get; }
        public ReactiveProperty<MColor> AccentColor { get; }
        public ReactiveProperty<MColor> SubColor { get; }

        public ReactiveProperty<BitmapImage> Artwork { get; }

        public ReactiveProperty<string> Title { get; }
        public ReactiveProperty<string> Album { get; }
        public ReactiveProperty<string> Artist { get; }

        public ReactiveProperty<MColor> InvertedMainColor { get; }

        public MainWindowViewModel()
        {
            _iTunes = new iTunes().AddTo(CompositeDisposable);

            var currentTrack = Observable.Interval(TimeSpan.FromSeconds(0.5))
                                         .Select(_ => _iTunes.GetCurrentTrack())
                                         .Where(track => track != null)
                                         .DistinctUntilChanged()
                                         .Publish();

            Title = currentTrack.Select(x => x.Name).ToReactiveProperty();
            Album = currentTrack.Select(x => x.Album).ToReactiveProperty();
            Artist = currentTrack.Select(x => x.Artist).ToReactiveProperty();

            var artwork = currentTrack.Select(x => x.Artwork.FirstOrDefault())
                                      .Where(x => x != null)
                                      .DistinctUntilChanged();

            _colors = artwork.SelectMany(x => x.GetStreamAsync())
                             .Select(x => new Bitmap(x))
                             .Select(Analyzer.GetColors);

            MainColor = _colors.Select(x => x.Item1.ToMediaColor()).ToReactiveProperty();
            AccentColor = _colors.Select(x => x.Item2.ToMediaColor()).ToReactiveProperty();
            SubColor = _colors.Select(x => x.Item3.ToMediaColor()).ToReactiveProperty();

            Artwork = artwork.SelectMany(x => x.GetStreamAsync())
                             .Select(x =>
                             {
                                 var image = x.ToBitmapImage();
                                 image.Freeze();
                                 return image;
                             })
                             .ToReactiveProperty();

            InvertedMainColor = MainColor.CombineLatest(AccentColor, (main, accent) =>
            {
                var mainLuminance = Hsb.GetLuminance(main.ToDrawingColor());
                var accentHsb = Hsb.FromColor(accent.ToDrawingColor());
                var b = mainLuminance > 50 ? 0.1f : 0.9f;
                return accentHsb.SetBrightness(b).ToColor().ToMediaColor();

            }).ToReactiveProperty();

            currentTrack.Connect().AddTo(CompositeDisposable);
        }

        public async void Initialize()
        {
            var bridge = new Bridge("192.168.0.15")
            {
                UserName = "265c6606152d58042d87d6524ca2cff"
            };

            var lights = await bridge.GetLightsAsync(CancellationToken.None);

            _colors.Subscribe(async x => await new[]
            {
                lights.ElementAt(0).SetXyAsync(x.Item1.ToXy()),
                lights.ElementAt(1).SetXyAsync(x.Item2.ToXy()),
                lights.ElementAt(2).SetXyAsync(x.Item1.ToXy()),
                lights.ElementAt(3).SetXyAsync(x.Item3.ToXy()),

            }.WhenAll());
        }
    }
}

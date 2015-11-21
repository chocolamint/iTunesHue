using iTunesHue.ViewModels;
using iTunesHue.Views;
using System.Windows;

namespace iTunesHue
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var window = new MainWindow
            {
                DataContext = new MainWindowViewModel()
            };

            window.ShowDialog();
        }
    }
}

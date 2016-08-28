using System.Windows;
using MatrixIO.IO;

namespace TsViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private PortabilityFactory _pf;

        public App()
        {
            _pf = new MpegTsPortabilityFactory(Dispatcher);
        }

        private void ApplicationExit(object sender, ExitEventArgs e)
        {
            TsViewer.Properties.Settings.Default.Save();
        }
    }
}

using System.Windows;
using MatrixIO.IO;

namespace BmffViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private PortabilityFactory _pf = new BmffPortabilityFactory();

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            BmffViewer.Properties.Settings.Default.Save();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QTOControlLib;

namespace QTCommon
{
    public static class Converter
    {

        public static void Convert(string profilePath, string url, string output)
        {
            if (!url.StartsWith("http"))
            {
                url = Path.GetFullPath(url);
            }

            output = Path.GetFullPath(output);

            Console.WriteLine("Source {0}", url);
            Console.WriteLine("Destination {0}", output);

            //var qt = axQTControl1;
            var form = new Form();
            // show the form so quicktime bloody works!
            form.Show();

            var qt = new AxQTOControlLib.AxQTControl();
            qt.Parent = form;
            qt.URL = url;

            var qtContainer = qt.QuickTime;
            qtContainer.Exporters.Add();
            var exporter = qtContainer.Exporters[1];
            exporter.SetDataSource(qt.Movie);
            exporter.ShowProgressDialog = false;
            exporter.DestinationFileName = output;
            exporter.TypeName = "QuickTime Movie";
            //exporter.ShowSettingsDialog();

            var settings = new QTOLibrary.CFObject();
            settings.XML = File.ReadAllText(profilePath);
            exporter.Settings = settings;

            exporter.EventListeners.Add(QTOLibrary.QTEventClassesEnum.qtEventClassProgress, QTOLibrary.QTEventIDsEnum.qtEventExportProgress);
            qt.QTEvent += Qt_QTEvent;

            form.Hide();
            exporter.BeginExport();
            

            qt.Dispose();

            form.Close();
            form.Dispose();
        }

        private static void Qt_QTEvent(object sender, AxQTOControlLib._IQTControlEvents_QTEventEvent e)
        {
            if (e.eventID == (int)QTOLibrary.QTEventIDsEnum.qtEventExportProgress)
            {
                // log progress
                var progress = (float)e.eventObject.GetParam(QTOLibrary.QTEventObjectParametersEnum.qtEventParamAmount);

                // At the end the paramAmount = 0 (for some reason), if so make it 100!
                if (progress == 1)
                    progress = 100;
               // var line = Console.CursorTop;
                Console.WriteLine(progress);
                //Console.CursorTop = line;
            }
        }

        public static void EditSettings(string settingsXmlPath)
        {
            string settingsString = null;
            if (File.Exists(Path.GetFullPath(settingsXmlPath)))
            {
                settingsXmlPath = Path.GetFullPath(settingsXmlPath);
                settingsString = File.ReadAllText(settingsXmlPath);
            }

            var form = new Form();
            // show the form so quicktime bloody works!
            form.Show();

            var qt = new AxQTOControlLib.AxQTControl();
            qt.Parent = form;
            
            qt.URL = Path.GetFullPath("PlaceHolder.mp4");

            var qtContainer = qt.QuickTime;
            qtContainer.Exporters.Add();
            var exporter = qtContainer.Exporters[1];
            exporter.SetDataSource(qt.Movie);

            exporter.DestinationFileName = Path.GetFullPath("placeholder.mov");
            exporter.TypeName = "QuickTime Movie";

            if (!string.IsNullOrEmpty(settingsString))
            {
                var settings = new QTOLibrary.CFObject();
                settings.XML = File.ReadAllText(settingsXmlPath);
                exporter.Settings = settings;
            }

            exporter.ShowSettingsDialog();

            File.WriteAllText(settingsXmlPath, exporter.Settings.XML);
            qt.Dispose();
            form.Close();
            form.Dispose();
        }
    }
}

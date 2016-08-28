using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using MatrixIO.IO.Bmff;
using Microsoft.Win32;
using System.IO;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using MatrixIO.IO.Bmff.Boxes;

namespace BmffViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<IMediaFile> _files = new ObservableCollection<IMediaFile>();
        public ObservableCollection<IMediaFile> Files { get { return _files; } }

        public VirtualizedBinaryReader BinaryReader;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SupportedBoxes_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var supportedBoxes = new SupportedBoxesWindow();
            supportedBoxes.ShowDialog();
        }

        private static readonly string FileFormats = String.Join("|", new string[]
        {
            "All Supported Files|*.mp4;*.m4a;*.m4v;*.mov;*.moov;*.qif;*.qtif;*.ismv;*.isma;*.ismt;*.3gp;*.3gpp;*.3g2;*.3gpp2;*.dcf;*.m21;*.mp21;*.dvb;*.jp2;*.jpx;*.f4v;*.f4p;*.f4a;*.f4b",

            "Audio Files|*.m4a;*.isma;*.f4a",
            "Caption Files|*.ismt",
            "Mobile Files|*.3gp;*.3gpp;*.3g2;*.3gpp2;*.dcf",
            "Image Files|*.jp2;*.jpx;*.qif;*.qtif",
            "Video files|*.mp4;*.m4a;*.m4v;*.mov;*.moov;*.ismv;*.isma;*.ismt;*.3gp;*.3gpp;*.3g2;*.3gpp2;*.m21;*.mp21;*.dvb;*.f4v;*.f4p",
            
            "3GPP|*.3gp;*.3gpp;*.3g2;*.3gpp2",
            "DCF|*.dcf",
            "DVB|*.dvb",
            "JPEG2000|*.jp2;*.jpx;*.jpm",
            "MPEG|*.mp4;*.m4a;*.m4v;*.m21;*.mp21",
            "PIFF|*.ismv;*.isma;*.ismt",
            "Flash|*.f4v;*.f4p;*.f4a;*.f4b",

            "All Files|*.*",
        });

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog {Filter = FileFormats, CheckFileExists = true, Multiselect = true};
            openFileDialog.FileOk += (source, cancelEventArgs) =>
            {
                foreach (var bmf in from fileName in openFileDialog.FileNames where !Files.Any(file => file.FullName == fileName ? true : false) select new BaseMediaFile(fileName))
                {
                    Files.Add(bmf);
                }
            };
            openFileDialog.ShowDialog(this);
            e.Handled = true;
        }

        private void Close_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var item = TreeView1.Tag as TreeViewItem;
            if (item == null) return;

            BaseMediaFile bmf = GetFileForItem(item);
            if (bmf == null) return;

            e.CanExecute = true;
            e.Handled = true;
        }
        private void Close_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var item = TreeView1.Tag as TreeViewItem;
            if (item == null) return;

            BaseMediaFile bmf = GetFileForItem(item);
            if (bmf == null) return;

            Files.Remove(bmf);
            bmf.Close();
            bmf.Dispose();
            e.Handled = true;
        }

        private void SaveAs_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var item = TreeView1.Tag as TreeViewItem;
            if (item == null) return;

            BaseMediaFile bmf = GetFileForItem(item);
            if (bmf == null) return;

            e.CanExecute = true;
            e.Handled = true;
        }
        private void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var item = TreeView1.Tag as TreeViewItem;
            if (item == null) return;

            BaseMediaFile bmf = GetFileForItem(item);
            if (bmf == null) return;

            var saveFileDialog = new SaveFileDialog
                                     {
                                         FileName = bmf.NameWithoutExtension + " (Copy)" + bmf.Extension,
                                         InitialDirectory = bmf.DirectoryName,
                                         Filter = FileFormats,
                                         CheckFileExists = false
                                     };
            saveFileDialog.FileOk += (source, cancelEventArgs) =>
                                         {
                                             if (saveFileDialog.FileName != bmf.FullName)
                                             {
                                                 bmf.SaveAs(saveFileDialog.FileName);
                                                 Files.Add(new BaseMediaFile(saveFileDialog.FileName));
                                             }
                                             else
                                                 MessageBox.Show("Cannot Save As to the same filename as the source file.", "Error");
                                         };

            saveFileDialog.ShowDialog(this);
            e.Handled = true;
        }

        private void FastStart_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var item = TreeView1.Tag as TreeViewItem;
            if (item == null) return;
            BaseMediaFile bmf = GetFileForItem(item);
            if (bmf == null) return;
            var moov = bmf.Children.OfType<MovieBox>().SingleOrDefault();
            if (moov == null) return;
            var mdat = bmf.Children.OfType<MovieDataBox>().SingleOrDefault();
            if (mdat == null) return;

            if (bmf.Children.IndexOf(moov) > bmf.Children.IndexOf(mdat)) e.CanExecute = true;
            else
            {
                bool foundJunk = false;
                for (int i = bmf.Children.IndexOf(moov) + 1; i < bmf.Children.IndexOf(mdat); i++)
                {
                    if (bmf.Children[i].Type == "junk" || bmf.Children[i].Type == "skip" || bmf.Children[i].Type == "free" || bmf.Children[i].Type == "wide")
                        foundJunk = true;
                }
                if (!foundJunk) return;
                e.CanExecute = true;
            }
            e.Handled = true;
        }

        private void FastStart_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var item = TreeView1.Tag as TreeViewItem;
            if (item == null) return;

            BaseMediaFile bmf = GetFileForItem(item);
            if (bmf == null) return;

            var saveFileDialog = new SaveFileDialog
                                     {
                                         FileName = bmf.NameWithoutExtension + ".Progressive" + bmf.Extension,
                                         InitialDirectory = bmf.DirectoryName,
                                         Filter = FileFormats,
                                         CheckFileExists = false
                                     };
            saveFileDialog.FileOk += (source, cancelEventArgs) =>
                                         {
                                             if (saveFileDialog.FileName != bmf.FullName)
                                             {
                                                 bmf.FastStart(saveFileDialog.FileName);
                                                 Files.Add(new BaseMediaFile(saveFileDialog.FileName));
                                             }
                                             else
                                                 MessageBox.Show("Cannot FastStart to the same filename as the source file.", "Error");
                                         };

            saveFileDialog.ShowDialog(this);
            e.Handled = true;
        }

        private void DecompressMovieHeader_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var item = TreeView1.Tag as TreeViewItem;
            if (item == null) return;

            BaseMediaFile bmf = GetFileForItem(item);
            if (bmf == null) return;
            var moov = bmf.Children.OfType<MovieBox>().SingleOrDefault();
            if (moov == null) return;
            var cmov = moov.Children.OfType<CompressedMovieBox>().SingleOrDefault();
            if (cmov == null) return;
            var dcom = cmov.Children.OfType<DataCompressionBox>().SingleOrDefault();
            if (dcom == null) return;
            var cmvd = cmov.Children.OfType<CompressedMovieDataBox>().SingleOrDefault();
            if (cmvd == null) return;
            
            e.CanExecute = true;
            e.Handled = true;
        }

        private void DecompressMovieHeader_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var item = TreeView1.Tag as TreeViewItem;
            if (item == null) return;
            BaseMediaFile bmf = GetFileForItem(item);
            if (bmf == null) return;
            bmf.DecompressMovieHeader();
            e.Handled = true;
        }

        // This is a bit hackish but it saves the TreeViewItem for the current selected item.
        private void TreeView1_TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            var tv = sender as TreeView;
            if (tv != null) tv.Tag = e.OriginalSource;
        }

        private BaseMediaFile GetFileForItem(TreeViewItem item)
        {
            var currentItem = item;
            while(currentItem != null)
            {
                var parent = GetTreeViewItemParent(currentItem);
                if (parent !=null) currentItem = parent;
                else break;
            }
            var treeViewItem = (TreeViewItem) currentItem;
            return (treeViewItem != null) ? treeViewItem.DataContext as BaseMediaFile : null;
        }

        private TreeViewItem GetTreeViewItemParent(TreeViewItem item)
        {
            var parent = VisualTreeHelper.GetParent(item);
            while (parent != null && !(parent is TreeViewItem || parent is TreeView))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as TreeViewItem;
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.DataContext == CollectionView.NewItemPlaceholder ? "*" : e.Row.GetIndex().ToString(CultureInfo.InvariantCulture);
        }

        private LogWindow _logWindow;
        private void Log_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_logWindow == null)
            {
                _logWindow = new LogWindow();
                _logWindow.Closed += (sender2, e2) => { _logWindow = null; };
                _logWindow.Show();
            }
            else
            {
                _logWindow.Activate();
            }
        }
    }
}

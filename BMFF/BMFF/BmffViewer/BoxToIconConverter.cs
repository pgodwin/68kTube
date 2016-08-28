using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;
using System.Reflection;
using MatrixIO.IO.Bmff.Boxes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MatrixIO.IO.Bmff;
using System.Windows.Interop;
using System.Drawing;
using System.Windows;

namespace BmffViewer
{
    public class BoxToIconConverter : MarkupExtension, IValueConverter
    {
        public BoxToIconConverter() : base() { }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is BaseMediaFile) return GetFileIcon(((BaseMediaFile)value).FullName);
            if (value is UnknownBox) return new BitmapImage(new Uri("pack://application:,,,/BmffViewer;component/Images/109_AllAnnotations_Warning_16x16_72.png"));         
            if(value is ISuperBox) return new BitmapImage(new Uri("pack://application:,,,/BmffViewer;component/Images/Folder_16x16.png"));
            return new BitmapImage(new Uri("pack://application:,,,/BmffViewer;component/Images/EntityDataModel_ComplexTypeProperty_16x16.png"));
        }

        private ImageSource GetFileIcon(string filename)
        {

            using (Icon i = Icon.ExtractAssociatedIcon(filename))
            {
                return Imaging.CreateBitmapSourceFromHIcon(
                                        i.Handle,
                                        new Int32Rect(0, 0, i.Width, i.Height),
                                        BitmapSizeOptions.FromEmptyOptions());
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Data;
using MatrixIO.IO.Bmff;

namespace BmffViewer
{
    public class BoxToHexConverter : MarkupExtension, IValueConverter
    {
        public BoxToHexConverter() : base() { }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Box box = value as Box;
            if(box!=null && box.HasContent)
                return new VirtualizedBinaryReader(box.GetContentStream(), (long)box.ContentOffset.Value, (long)box.ContentSize.Value);

            return null;
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

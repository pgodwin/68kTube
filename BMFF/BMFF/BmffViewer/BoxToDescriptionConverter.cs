using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Data;
using MatrixIO.IO.Bmff;
using MatrixIO.IO.Bmff.Boxes;

namespace BmffViewer
{
    public class BoxToDescriptionConverter : MarkupExtension, IValueConverter
    {
        public BoxToDescriptionConverter() : base() { }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is UnknownBox) return "Unknown Box Type";
            else if (value is Box)
            {
                // TODO: This may have to change for Sample Description Box ('stsd') children.
                object[] boxAttributes = value.GetType().GetCustomAttributes(typeof(BoxAttribute), true);
                foreach (BoxAttribute boxAttribute in boxAttributes)
                    if (((Box)value).Type == boxAttribute.Type) return boxAttribute.Description;

                // The BoxType doesn't match any of the BoxAttributes associated with this object.  Eek!
                return "Badly Defined Box Type";
            }
            else return "";
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

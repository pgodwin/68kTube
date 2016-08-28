using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Data;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using MatrixIO.IO.Bmff;

namespace BmffViewer
{
    public class ObjectPropertiesToCollectionViewConverter : MarkupExtension, IValueConverter
    {
        public ObjectPropertiesToCollectionViewConverter() : base() { }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            List<ObjectProperty> objectProperties = new List<ObjectProperty>();
            if (value != null)
            {
                foreach (PropertyInfo property in value.GetType().GetProperties())
                {
                    if (value is ISuperBox && property.Name == "Children") continue;
                    if (value is ITableBox && property.Name == "Entries") continue;
                    if ((value is Box || value is IMediaFile) && property.Name == "SourceStream") continue;

                    object propertyValue = property.GetValue(value, null);

                    IEnumerable propertyValues = propertyValue as IEnumerable;
                    if (propertyValues != null && !(propertyValues is String))
                    {
                        List<string> stringValues = new List<string>();
                        foreach (object item in propertyValues) stringValues.Add(item.ToString());
                        objectProperties.Add(new ObjectProperty() { Name = property.Name, Value = String.Join(", ", stringValues), Class = property.DeclaringType.Name });
                    }
                    else objectProperties.Add(new ObjectProperty() { Name = property.Name, Value = propertyValue != null ? propertyValue.ToString() : null, Class = property.DeclaringType.Name });
                }
            }
            ICollectionView viewSource = CollectionViewSource.GetDefaultView(objectProperties);
            viewSource.GroupDescriptions.Add(new PropertyGroupDescription("Class"));
            return viewSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public class ObjectProperty
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public string Class { get; set; }
        }
    }
}

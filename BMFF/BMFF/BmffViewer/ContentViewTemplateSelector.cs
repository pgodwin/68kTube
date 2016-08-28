using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Collections;
using MatrixIO.IO.Bmff;

namespace BmffViewer
{
    public class ContentViewTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Binary { get; set; }
        public DataTemplate Table { get; set; }

        public override DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            if (item is ITableBox) return Table;
            else return Binary;
        }
    }
}

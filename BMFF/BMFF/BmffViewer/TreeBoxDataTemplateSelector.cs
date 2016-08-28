using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using MatrixIO.IO.Bmff;
using System.Collections;

namespace BmffViewer
{
    public class TreeBoxDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Node { get; set; }
        public DataTemplate Leaf { get; set; }

        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            if (item is IEnumerable) return Node;
            else return Leaf;
        }
    }
}

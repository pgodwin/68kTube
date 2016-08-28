using System.Collections;
using System.Windows;
using System.Windows.Controls;
using MatrixIO.IO.MpegTs;

namespace TsViewer
{
    public class TreeBoxDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SourceNode { get; set; }
        public DataTemplate ProgramNode { get; set; }
        public DataTemplate StreamNode { get; set; }
        public DataTemplate Node { get; set; }
        public DataTemplate Leaf { get; set; }

        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            if (item is TsSource) return SourceNode;
            else if (item is TsProgram) return ProgramNode;
            else if (item is TsStream) return StreamNode;
            else if (item is IEnumerable) return Node;
            else return Leaf;
        }
    }
}

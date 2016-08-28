using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using MatrixIO.IO;

namespace BmffViewer
{
    internal class BmffPortabilityFactory : PortabilityFactory
    {
        #region List Creation
        public override IList<T> CreateList<T>()
        {
            return new ObservableCollection<T>();
        }
        public override IList<T> CreateList<T>(IEnumerable<T> collection)
        {
            return new ObservableCollection<T>(collection);
        }
        #endregion

        #region Tracing
        public override void TraceWriteLine(object value, string category = null)
        {
            Trace.WriteLine(value, category);
        }

        public override void TraceAssert(bool condition, string message = null)
        {
            Trace.Assert(condition, message);

        }

        public override int TraceIndentLevel
        {
            get
            {
                return Trace.IndentLevel;
            }
            set
            {
                Trace.IndentLevel = value;
            }
        }
        #endregion
    }
}

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Threading;
using MatrixIO.Collections;
using MatrixIO.IO;

namespace TsViewer
{
    internal class MpegTsPortabilityFactory : PortabilityFactory
    {
        private readonly Dispatcher _dispatcher;

        public MpegTsPortabilityFactory(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;    
        }

        #region List Creation

        public override IList<T> CreateList<T>()
        {
            return new DispatchingObservableList<T>();
        }
        public override IList<T> CreateList<T>(int capacity)
        {
            return new DispatchingObservableList<T>(capacity);
        }
        public override IList<T> CreateList<T>(IEnumerable<T> collection)
        {
            return new DispatchingObservableList<T>(collection);
        }

        #endregion

        public override void DispatchAction(System.Action action)
        {
            _dispatcher.BeginInvoke(DispatcherPriority.DataBind, action);
        }

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

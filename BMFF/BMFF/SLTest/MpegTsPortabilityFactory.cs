using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using MatrixIO.Collections;
using MatrixIO.IO;

namespace SLTest
{
    internal class MpegTsPortabilityFactory : PortabilityFactory
    {

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

        private readonly Dispatcher _dispatcher = Deployment.Current.Dispatcher;
        public override void DispatchAction(System.Action action)
        {
            _dispatcher.BeginInvoke(action);
        }

        public override void TraceWriteLine(object value, string category = null)
        {
           
        }
    }
}

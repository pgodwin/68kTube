using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Threading;
using System.Collections.Specialized;
using System.ComponentModel;

namespace TsViewer
{

    /// <summary>  
    /// ObservableCollection that automatically calls event handlers via the target object's Dispatcher if available.
    /// Targets that don't derive from DispatcherObject are called on the same thread that the event is invoked from.
    /// 
    /// Author: Jason S. Clary
    /// </summary>  
    /// <typeparam name="T">The type of the elements</typeparam>  
    public class DispatchingObservableCollection<T> : ObservableCollection<T>
    {
        private readonly EventHandlerList _delegates = new EventHandlerList();
        private readonly Dictionary<Dispatcher, EventHandlerList> _dispatcherDelegates =
            new Dictionary<Dispatcher, EventHandlerList>();

        public DispatchingObservableCollection() { }
        public DispatchingObservableCollection(IEnumerable<T> collection) : base(collection) { }

        private readonly object _collectionChanged = new Object();
        public override event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { AddEventHandler(_collectionChanged, value); }
            remove { RemoveEventHandler(_collectionChanged, value); }
        }

        private readonly object _propertyChanged = new Object();
        protected override event PropertyChangedEventHandler PropertyChanged
        {
            add { AddEventHandler(_propertyChanged, value); }
            remove { RemoveEventHandler(_propertyChanged, value); }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var threadAgnosticDelegates = (NotifyCollectionChangedEventHandler)_delegates[_collectionChanged];
            if (threadAgnosticDelegates != null) threadAgnosticDelegates(this, e);

            foreach (var dispatcherDelegatePair in _dispatcherDelegates)
            {
                var eventHandler =
                    (NotifyCollectionChangedEventHandler)dispatcherDelegatePair.Value[_collectionChanged];
                if (eventHandler == null) continue;
                if (dispatcherDelegatePair.Key.CheckAccess())
                    eventHandler(this, e);
                else
                    dispatcherDelegatePair.Key.BeginInvoke(DispatcherPriority.DataBind, new Action(() => eventHandler(this, e)));
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var threadAgnosticDelegates = (PropertyChangedEventHandler)_delegates[_propertyChanged];
            if (threadAgnosticDelegates != null) threadAgnosticDelegates(this, e);

            foreach (var dispatcherDelegatePair in _dispatcherDelegates)
            {
                var eventHandler =
                    (PropertyChangedEventHandler)dispatcherDelegatePair.Value[_propertyChanged];
                if (eventHandler == null) continue;
                if (dispatcherDelegatePair.Key.CheckAccess())
                    eventHandler(this, e);
                else
                    dispatcherDelegatePair.Key.BeginInvoke(DispatcherPriority.DataBind, new Action(() => eventHandler(this, e)));
            }
        }

        private void AddEventHandler(object eventObject, Delegate handler)
        {
            var target = handler.Target as DispatcherObject;
            if (target != null)
            {
                EventHandlerList eventHandlerList;
                if (!_dispatcherDelegates.TryGetValue(target.Dispatcher, out eventHandlerList))
                {
                    eventHandlerList = new EventHandlerList();
                    _dispatcherDelegates.Add(target.Dispatcher, eventHandlerList);
                }
                eventHandlerList.AddHandler(eventObject, handler);
            }
            else
                _delegates.AddHandler(eventObject, handler);
        }

        private void RemoveEventHandler(object eventObject, Delegate handler)
        {
            var target = handler.Target as DispatcherObject;
            if (target != null)
            {
                EventHandlerList eventHandlerList;
                if (!_dispatcherDelegates.TryGetValue(target.Dispatcher, out eventHandlerList))
                {
                    eventHandlerList = new EventHandlerList();
                    _dispatcherDelegates.Add(target.Dispatcher, eventHandlerList);
                }
                eventHandlerList.RemoveHandler(eventObject, handler);
            }
            else
                _delegates.RemoveHandler(eventObject, handler);
        }
    }
}

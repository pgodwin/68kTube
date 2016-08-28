using System;
using System.Collections;
#if !SILVERLIGHT
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
#if SILVERLIGHT
using System.Windows;
#endif
using System.Windows.Threading;

namespace MatrixIO.Collections
{
    /// <summary>  
    /// Observable List<typeparamref name="T"/> that automatically calls event handlers via the target object's Dispatcher if available.
    /// Targets that don't derive from DispatcherObject in WPF or DependencyObject in SilverLight are called on the same thread that 
    /// the event is invoked from.
    /// 
    /// Author: Jason S. Clary
    /// </summary>  
    /// <typeparam name="T">The type of the elements</typeparam>  
    public class DispatchingObservableList<T> : IList<T>, ICollection, INotifyCollectionChanged, INotifyPropertyChanged
    {
        public DispatchingObservableList()
        {
            _collection = new List<T>();
        }
        public DispatchingObservableList(int capacity)
        {
            _collection = new List<T>(capacity);
        }
        public DispatchingObservableList(IEnumerable<T> collection)
        {
            _collection = new List<T>(collection);
        }

        #region ICollection Implementation
        private readonly object _syncObject = new object();

        public void CopyTo(System.Array array, int index)
        {
            throw new System.NotImplementedException();
        }

        public bool IsSynchronized
        {
            get { return true; }
        }

        public object SyncRoot
        {
            get { return _syncObject; }
        }
        #endregion

        #region IList<T> Implementation
        private readonly List<T> _collection;

        public int IndexOf(T item)
        {
            lock (_syncObject) return _collection.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            lock (_syncObject)
            {
                _collection.Insert(index, item);
                OnNotifyPropertyChanged("Count");
                OnNotifyCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
            }
        }

        public void RemoveAt(int index)
        {
            lock(_syncObject)
            {
                var item = _collection[index];
                _collection.RemoveAt(index);
                OnNotifyPropertyChanged("Count");
                OnNotifyCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
            }
        }

        public T this[int index]
        {
            get
            {
                lock(_syncObject) return _collection[index];
            }
            set
            {
                lock (_syncObject)
                {
                    // TODO: Handle possible insert scenario?
                    var oldItem = _collection[index];
                    _collection[index] = value;
                    OnNotifyCollectionChanged(NotifyCollectionChangedAction.Replace, value, oldItem, index);
                }
            }
        }

        public void Add(T item)
        {
            lock(_syncObject)
            {
                _collection.Add(item);
                var index = _collection.IndexOf(item);
                OnNotifyPropertyChanged("Count");
                OnNotifyCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
            }
        }

        public void Clear()
        {
            lock(_syncObject)
            {
                _collection.Clear();
                OnNotifyPropertyChanged("Count");
                OnNotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
            }
        }

        public bool Contains(T item)
        {
            lock(_syncObject) return _collection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock(_syncObject) _collection.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { lock (_syncObject) return _collection.Count; }
        }

        public bool IsReadOnly
        {
            get { return ((IList<T>) _collection).IsReadOnly; }
        }

        public bool Remove(T item)
        {
            lock(_syncObject)
            {
                var index = _collection.IndexOf(item);
                if (_collection.Remove(item))
                {
                    OnNotifyPropertyChanged("Count");
                    OnNotifyCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
                    return true;
                }
                return false;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock(_syncObject)
            {
                foreach (var item in _collection)
                    yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Dispatching Event Handler Impelementation

        private readonly EventHandlerList _delegates = new EventHandlerList();
#if SILVERLIGHT
        private readonly Dispatcher _dispatcher = Deployment.Current.Dispatcher;
#else
        private readonly ConcurrentDictionary<Dispatcher, EventHandlerList> _dispatcherDelegates = new ConcurrentDictionary<Dispatcher, EventHandlerList>();
#endif
        private void AddEventHandler(object eventObject, Delegate handler)
        {
#if !SILVERLIGHT
            var target = handler.Target as DispatcherObject;
            if (target != null)
            {
                EventHandlerList eventHandlerList;
                if (!_dispatcherDelegates.TryGetValue(target.Dispatcher, out eventHandlerList))
                {
                    eventHandlerList = new EventHandlerList();
                    eventHandlerList.AddHandler(eventObject, handler);
                    _dispatcherDelegates.AddOrUpdate(target.Dispatcher, eventHandlerList, (key, oldValue) =>
                                                                                              {
                                                                                                  oldValue.AddHandlers(
                                                                                                      eventHandlerList);
                                                                                                  return oldValue;
                                                                                              });

                }
                else eventHandlerList.AddHandler(eventObject, handler);
            }
            else
#endif
                _delegates.AddHandler(eventObject, handler);
        }

        private void RemoveEventHandler(object eventObject, Delegate handler)
        {
#if !SILVERLIGHT
            var target = handler.Target as DispatcherObject;
            if (target != null)
            {
                EventHandlerList eventHandlerList;
                if (!_dispatcherDelegates.TryGetValue(target.Dispatcher, out eventHandlerList))
                    throw new ArgumentException();
                eventHandlerList.RemoveHandler(eventObject, handler);
            }
            else
#endif
                _delegates.RemoveHandler(eventObject, handler);
        }
        #endregion

        #region INotifyPropertyChanged Implementation
        private readonly object _propertyChanged = new Object();
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { AddEventHandler(_propertyChanged, value); }
            remove { RemoveEventHandler(_propertyChanged, value); }
        }

        void OnNotifyPropertyChanged(string propertyName)
        {
            OnNotifyPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
        void OnNotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            var delegates = (PropertyChangedEventHandler)_delegates[_propertyChanged];
            if (delegates != null)
#if SILVERLIGHT
                _dispatcher.BeginInvoke(() => delegates(this, e));
#else 
                delegates(this, e);

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
#endif
        }
        #endregion

        #region INotifyCollectionChanged Implementation
        private readonly object _collectionChanged = new Object();
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { AddEventHandler(_collectionChanged, value); }
            remove { RemoveEventHandler(_collectionChanged, value); }
        }

        void OnNotifyCollectionChanged(NotifyCollectionChangedAction action)
        {
            OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(action));
        }
        void OnNotifyCollectionChanged(NotifyCollectionChangedAction action, object changedItem, int index)
        {
            OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(action, changedItem, index));
        }
        void OnNotifyCollectionChanged(NotifyCollectionChangedAction action, object newItem, object oldItem, int index)
        {
            OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
        }
        void OnNotifyCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var delegates = (NotifyCollectionChangedEventHandler)_delegates[_collectionChanged];
            if (delegates != null)
#if SILVERLIGHT
                _dispatcher.BeginInvoke(() => delegates(this, e));
#else 
                delegates(this, e);

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
#endif
        }
        #endregion
    }

#if SILVERLIGHT
    // SilverLight doesn't have an EventHandlerList collection
    public class EventHandlerList
    {
        private readonly Dictionary<object, Delegate> _dictionary = new Dictionary<object, Delegate>();

        public Delegate this[object key]
        {
            get
            {
                Delegate d;
                _dictionary.TryGetValue(key, out d);
                return d;
            }
            set { AddHandler(key, value); }
        }

        public void AddHandler(object key, Delegate value)
        {
            Delegate d;
            if (!_dictionary.TryGetValue(key, out d)) _dictionary.Add(key, value);
            else
            {
                Delegate newDelegate = Delegate.Combine(d, value);
                _dictionary[key] = newDelegate;
            }
        }
        public void RemoveHandler(object key, Delegate value)
        {
            Delegate d;
            if (_dictionary.TryGetValue(key, out d)) _dictionary[key] = Delegate.Remove(d, value);
        }
    }
#endif
}

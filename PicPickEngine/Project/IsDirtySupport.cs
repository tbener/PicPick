using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.Project
{
    public class IsDirtySupport<T> : IDisposable where T : class, IIsDirtySupport
    {
        private T _mainClass;
        private bool _initialized;
        private bool _isDirty;

        public event EventHandler OnGotDirty;

        public IsDirtySupport(T rootClass)
        {
            Init(rootClass);
        }

        private void Init(T rootClass)
        {
            if (_initialized && _mainClass == rootClass)
                return;

            _mainClass = rootClass;

            if (rootClass.GetType().GetInterface("INotifyPropertyChanged") != null)
            {
                SubscribePropertyChangedObject(rootClass as INotifyPropertyChanged);
            }

            return;

            var props = rootClass.GetType().GetProperties();

            foreach (var prp in props)
            {
                //AddProperty(_mainClass, prp);

                //if (prp.PropertyType.GetInterface("IIsDirtySupport") != null)
                //{
                //    // if this propoerty implements IsDirty then we don't need to subscribe to any other PropertyChanged events.
                //    ((IIsDirtySupport)prp).OnGotDirty += (s, e) => SetIsDirty();
                //    continue;
                //}

                //if (prp.PropertyType.GetInterface("INotifyCollectionChanged") != null)
                //{
                //    ((INotifyCollectionChanged)prp).CollectionChanged += (s, e) =>
                //    {
                //        SetIsDirty();

                //        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems[0].GetType().GetInterface("INotifyPropertyChanged") != null)
                //        {
                //            foreach (var newItem in e.NewItems)
                //            {
                //                SubscribePropertyChangedObject(newItem as INotifyPropertyChanged);
                //            }
                //        }
                //    };
                //}
                //prp.GetType().inter
            }

            _initialized = true;
        }

        

        private void AddProperty(object parent, PropertyInfo prp)
        {
            if (prp.PropertyType.GetInterface("IIsDirtySupport") != null)
            {
                // if this propoerty implements IsDirty then we don't need to subscribe to any other PropertyChanged events.
                ((IIsDirtySupport)prp).GetIsDirtyInstance().OnGotDirty += (s, e) => IsDirty = true;
                return;
            }

            if (prp.PropertyType.GetInterface("INotifyCollectionChanged") != null)
            {
                // get existing items in collection
                var col = prp.GetValue(parent) as IEnumerable;

                // loop through existing objects and subscrive them
                foreach (var item in col)
                {
                    AddObject(item);
                }

                // monitor collection changed
                // for any new item added:
                // 1. SetDirty
                // 2. Subscribe it
                ((INotifyCollectionChanged)prp).CollectionChanged += (s, e) =>
                {
                    IsDirty = true;

                    if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems[0].GetType().GetInterface("INotifyPropertyChanged") != null)
                    {
                        foreach (var newItem in e.NewItems)
                        {
                            SubscribePropertyChangedObject(newItem as INotifyPropertyChanged);
                        }
                    }
                };
            }
        }

        private void AddObject(object item)
        {
            if (AddObject(item as IIsDirtySupport)) return;
            if (AddObject(item as INotifyPropertyChanged)) return;

            var props = item.GetType().GetProperties();
            foreach (var prp in props)
            {
                AddProperty(item, prp);
            }
        }

        private bool AddObject(INotifyPropertyChanged item)
        {
            if (item == null) return false;
            item.PropertyChanged += (s, e) => IsDirty = true;
            return true;
        }

        private bool AddObject(IIsDirtySupport item)
        {
            if (item == null) return false;
            item.GetIsDirtyInstance().OnGotDirty += (s, e) => IsDirty = true; 
            return true;
        }

        private void SubscribePropertyChangedObject(INotifyPropertyChanged cls)
        {
            cls.PropertyChanged += (s, e) =>
            {
                // not sure we need this
                if (e.PropertyName == "IsDirty")
                    return;

                IsDirty = true;
            };
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (value != _isDirty)
                {
                    _isDirty = value;
                    if (_isDirty)
                        OnGotDirty?.Invoke(_mainClass, null);
                }
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException(this.GetType().ToString());
        }
    }
}

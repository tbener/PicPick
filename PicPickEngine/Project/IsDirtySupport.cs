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
    [AttributeUsage(AttributeTargets.Property)]
    public class IsDirtyIgnoreAttribute : Attribute
    {

    }

    public class IsDirtySupport<T> : IDisposable where T : class, IIsDirtySupport
    {
        private T _mainClass;
        private bool _initialized;
        private bool _isDirty;
        private Dictionary<Type, List<string>> _ignoreList = new Dictionary<Type, List<string>>();

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

            AddObject(rootClass);

        }

        private void AddObject(object cls)
        {

            if (cls.GetType().GetInterface("INotifyPropertyChanged") != null)
            {
                SubscribePropertyChangedObject(cls as INotifyPropertyChanged);
            }

            var props = cls.GetType().GetProperties();

            foreach (var prp in props)
            {
                //AddProperty(_mainClass, prp);

                //if (prp.PropertyType.GetInterface("IIsDirtySupport") != null)
                //{
                //    // if this propoerty implements IsDirty then we don't need to subscribe to any other PropertyChanged events.
                //    ((IIsDirtySupport)prp).OnGotDirty += (s, e) => SetIsDirty();
                //    continue;
                //}

                if (HasIgnoreAttribute(cls, prp))
                    continue;


                if (prp.PropertyType.GetInterface("INotifyCollectionChanged") != null)
                {
                    // subscribe existing items
                    var collection = prp.GetValue(cls) as IEnumerable;

                    foreach (var item in collection)
                    {
                        AddObject(item);
                    }

                    // subscribe future items
                    ((INotifyCollectionChanged)prp.GetValue(cls)).CollectionChanged += (s, e) =>
                    {
                        IsDirty = true;

                        if (e.Action == NotifyCollectionChangedAction.Add)
                        {
                            foreach (var newItem in e.NewItems)
                            {
                                AddObject(newItem);
                            }
                        }
                    };

                    continue;
                }

                if (prp.PropertyType.Module.ScopeName == "CommonLanguageRuntimeLibrary")
                {
                    Console.WriteLine($"  Skipped: {cls.GetType().ToString()}.{prp.Name} ({prp.PropertyType})");
                    continue;
                }

                if (prp.PropertyType.IsArray || prp.PropertyType.IsEnum)
                    continue;

                if (prp.PropertyType.IsClass)
                {
                    if (prp.PropertyType.GetInterface("IIsDirtySupport") != null)
                        // if this propoerty implements IsDirty then we don't want to re-subscribe to all PropertyChanged events again.
                        ((IIsDirtySupport)prp).GetIsDirtyInstance().OnGotDirty += (s, e) => IsDirty = true;
                    else
                    {
                        Console.WriteLine($"** Calling for {cls.GetType().ToString()}.{prp.Name} ({prp.PropertyType})");
                        AddObject(prp.GetValue(cls));
                    }
                }

            }

            _initialized = true;
        }

        private bool HasIgnoreAttribute(object cls, PropertyInfo prp)
        {
            if (Attribute.IsDefined(prp, typeof(IsDirtyIgnoreAttribute)))
            {
                // add to ignore list
                if (!_ignoreList.ContainsKey(cls.GetType()))
                    _ignoreList.Add(cls.GetType(), new List<string>());
                if (!_ignoreList[cls.GetType()].Contains(prp.Name))
                    _ignoreList[cls.GetType()].Add(prp.Name);

                return true;
            }
            return false;
        }

        private void SubscribePropertyChangedObject(INotifyPropertyChanged cls)
        {
            cls.PropertyChanged += (s, e) =>
            {
                // check if in the ignore list
                if (_ignoreList.ContainsKey(s.GetType()) && _ignoreList[s.GetType()].Contains(e.PropertyName))
                    return;

                // not sure we need this
                if (e.PropertyName == "IsDirty")
                    return;

                IsDirty = true;
            };
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



        //private void AddObject1(object item)
        //{
        //    if (AddObject(item as IIsDirtySupport)) return;
        //    if (AddObject(item as INotifyPropertyChanged)) return;

        //    var props = item.GetType().GetProperties();
        //    foreach (var prp in props)
        //    {
        //        AddProperty(item, prp);
        //    }
        //}

        //private bool AddObject(INotifyPropertyChanged item)
        //{
        //    if (item == null) return false;
        //    item.PropertyChanged += (s, e) => IsDirty = true;
        //    return true;
        //}

        //private bool AddObject(IIsDirtySupport item)
        //{
        //    if (item == null) return false;
        //    item.GetIsDirtyInstance().OnGotDirty += (s, e) => IsDirty = true; 
        //    return true;
        //}


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

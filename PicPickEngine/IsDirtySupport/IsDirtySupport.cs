using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.Project.IsDirtySupport
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IsDirtyIgnoreAttribute : Attribute
    {

    }

    public class ObjectPropertiesDictionary<T, L> : Dictionary<T, L> where T : Type where L : List<string>, new()
    {
        public void Add(T obj, string prp)
        {
            if (!ContainsKey(obj))
                Add(obj, new L());
            if (!this[obj].Contains(prp))
                this[obj].Add(prp);
        }

        public bool Contains(T t, string prp)
        {
            return (ContainsKey(t) && this[t].Contains(prp));
        }


    }

    public class IsDirtySupport<T> : IDisposable where T : class, IIsDirtySupport
    {
        private T _mainClass;
        private bool _initialized;
        private bool _isDirty;

        private ObjectPropertiesDictionary<Type, List<string>> _ignoredProperties = new ObjectPropertiesDictionary<Type, List<string>>();
        private ObjectPropertiesDictionary<Type, List<string>> _monitoredProperties = new ObjectPropertiesDictionary<Type, List<string>>();

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

            AddInstance(rootClass);

        }

        private void AddInstance(object cls)
        {
            if (cls == null) return;

            if (cls.GetType().GetInterface("INotifyPropertyChanged") != null)
            {
                SubscribePropertyChangedObject(cls as INotifyPropertyChanged);
            }

            var props = cls.GetType().GetProperties();

            foreach (var prp in props)
            {

                try
                {


                    if (HasIgnoreAttribute(cls, prp))
                        continue;

                    if (prp.PropertyType.GetInterface("INotifyCollectionChanged") != null)
                    {
                        AddNotifyCollectionProperty(cls, prp);
                        continue;
                    }

                    if (PropertyTypeExclude(prp.PropertyType))
                        continue;

                    if (prp.PropertyType.IsClass)
                    {
                        // add to Monitored Properties
                        _monitoredProperties.Add(cls.GetType(), prp.Name);

                        if (prp.PropertyType.GetInterface("IIsDirtySupport") != null)
                            // if this property implements IsDirty then we don't want to re-subscribe to all PropertyChanged events again.
                            ((IIsDirtySupport)prp).GetIsDirtyInstance().OnGotDirty += (s, e) => SetDirty(s, new PropertyChangedEventArgs("GotDirty"));
                        else
                        {
                            Console.WriteLine($"** Calling for {cls.GetType().ToString()}.{prp.Name} ({prp.PropertyType})");
                            AddInstance(prp.GetValue(cls));
                        }
                    }

                }
                catch (IsDirtyException ex)
                {
                    throw new IsDirtyException($"{cls.GetType().Name}.{prp.Name}.{ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    throw new IsDirtyException($"{cls.GetType().Name}.{prp.Name}", ex);
                }
            }

            _initialized = true;
        }

        private void AddNotifyCollectionProperty(object cls, PropertyInfo prp)
        {
            // subscribe existing items
            var collection = prp.GetValue(cls) as IEnumerable;

            foreach (var item in collection)
            {
                AddInstance(item);
            }

            // subscribe future items
            ((INotifyCollectionChanged)prp.GetValue(cls)).CollectionChanged += (s, e) =>
            {
                SetDirty(s, e);

                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (var newItem in e.NewItems)
                    {
                        AddInstance(newItem);
                    }
                }
            };
        }

        private bool PropertyTypeExclude(Type propertyType)
        {
            if (propertyType.Module.ScopeName == "CommonLanguageRuntimeLibrary")
                return true;

            if (propertyType.IsArray || propertyType.IsEnum)
                return true;

            return false;
        }

        private bool HasIgnoreAttribute(object cls, PropertyInfo prp)
        {
            if (Attribute.IsDefined(prp, typeof(IsDirtyIgnoreAttribute)))
            {
                // add to ignore list
                _ignoredProperties.Add(cls.GetType(), prp.Name);

                return true;
            }
            return false;
        }

        private void SubscribePropertyChangedObject(INotifyPropertyChanged cls)
        {
            cls.PropertyChanged += (s, e) =>
            {
                // check if in the ignore list
                if (_ignoredProperties.Contains(s.GetType(), e.PropertyName))
                    return;

                // not sure we need this
                if (e.PropertyName == "IsDirty")
                    return;

                SetDirty(s, e);

                // check if in the monitor list
                if (_monitoredProperties.Contains(s.GetType(), e.PropertyName))
                    AddInstance(s.GetType().GetProperty(e.PropertyName).GetValue(s));
            };
        }


        private void SetDirty(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine($"+ Item Changed: {sender.GetType().Name}.{e.PropertyName}");
            IsDirty = true;
        }

        private void SetDirty(object sender, NotifyCollectionChangedEventArgs e)
        {
            Console.WriteLine($"+ Collection Changed: {sender.GetType().Name} -> {e.Action.ToString()}");
            IsDirty = true;
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

    public class IsDirtyException : Exception
    {
        public IsDirtyException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}

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
        private Dictionary<Type, List<string>> _ignoredProperties = new Dictionary<Type, List<string>>();
        private Dictionary<Type, List<string>> _monitoredProperties = new Dictionary<Type, List<string>>();

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
            if (cls == null) return;

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

                try
                {


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
                            SetDirty(s, e);

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
                        // add to Monitored Properties
                        if (!_monitoredProperties.ContainsKey(cls.GetType()))
                            _monitoredProperties.Add(cls.GetType(), new List<string>());
                        if (!_monitoredProperties[cls.GetType()].Contains(prp.Name))
                            _monitoredProperties[cls.GetType()].Add(prp.Name);

                        if (prp.PropertyType.GetInterface("IIsDirtySupport") != null)
                            // if this propoerty implements IsDirty then we don't want to re-subscribe to all PropertyChanged events again.
                            ((IIsDirtySupport)prp).GetIsDirtyInstance().OnGotDirty += (s, e) => SetDirty(s, new PropertyChangedEventArgs("GotDirty")); 
                        else
                        {
                            Console.WriteLine($"** Calling for {cls.GetType().ToString()}.{prp.Name} ({prp.PropertyType})");
                            AddObject(prp.GetValue(cls));
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

        private bool HasIgnoreAttribute(object cls, PropertyInfo prp)
        {
            if (Attribute.IsDefined(prp, typeof(IsDirtyIgnoreAttribute)))
            {
                // add to ignore list
                if (!_ignoredProperties.ContainsKey(cls.GetType()))
                    _ignoredProperties.Add(cls.GetType(), new List<string>());
                if (!_ignoredProperties[cls.GetType()].Contains(prp.Name))
                    _ignoredProperties[cls.GetType()].Add(prp.Name);

                return true;
            }
            return false;
        }

        private void SubscribePropertyChangedObject(INotifyPropertyChanged cls)
        {
            cls.PropertyChanged += (s, e) =>
            {
                // check if in the ignore list
                if (_ignoredProperties.ContainsKey(s.GetType()) && _ignoredProperties[s.GetType()].Contains(e.PropertyName))
                    return;

                // not sure we need this
                if (e.PropertyName == "IsDirty")
                    return;

                SetDirty(s, e);

                // check if in the monitor list
                if (_monitoredProperties.ContainsKey(s.GetType()) && _monitoredProperties[s.GetType()].Contains(e.PropertyName))
                    AddObject(s.GetType().GetProperty(e.PropertyName).GetValue(s));
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

    public class IsDirtyException : Exception {
        public IsDirtyException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}

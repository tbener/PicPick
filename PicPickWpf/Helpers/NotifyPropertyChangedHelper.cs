using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PicPickUI.Helpers
{
    public class NotifyPropertyChangedHelper
    {
        PropertyChangedEventHandler _propertyChangeHandler;

        public void Add(PropertyChangedEventHandler value)
        {
            _propertyChangeHandler += value;
        }

        public void Remove(PropertyChangedEventHandler value)
        {
            _propertyChangeHandler -= value;
        }

        public void NotifyPropertyChanged(object sender, string propertyName)
        {
            if (_propertyChangeHandler != null)
                _propertyChangeHandler(sender, new PropertyChangedEventArgs(propertyName));
        }

        public void SetValue<T>(object containingObject, ref T field, T value, params string[] names)
        {
            if (names == null)
                throw new ArgumentNullException("names");

            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                for (int i = 0; i < names.Length; i++)
                {
                    NotifyPropertyChanged(containingObject, names[i]);
                }
            }
        }
    }
}

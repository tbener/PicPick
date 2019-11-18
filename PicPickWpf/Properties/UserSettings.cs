using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.Properties
{
    public class UserSettingsBase : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    internal partial class UserSettings
    {
        internal UserSettings()
        {
            General.PropertyChanged += Settings_PropertyChanged;
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //string settingsKey = sender.GetType().GetProperty(e.PropertyName).GetCustomAttribute<UserSettingsAttribute>().SettingsKey;
            if (AutoSave)
                Save();
        }

        internal static GeneralUserSettings General { get; } = new GeneralUserSettings();

        internal static bool AutoSave { get; set; }

        

    }

    public class GeneralUserSettings : UserSettingsBase
    {
        //[UserSettings("WarnDeleteSource")]
        public bool WarnDeleteSource
        {
            get { return UserSettings.Default.WarnDeleteSource; }
            set
            {
                UserSettings.Default.WarnDeleteSource = value;
                RaisePropertyChanged(nameof(WarnDeleteSource));
            }
        }

    }

    public class TestUserSettings
    {

    }

    //[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    //public class UserSettingsAttribute : Attribute
    //{
    //    public string SettingsKey { get; private set; }

    //    public UserSettingsAttribute(string settingsKey)
    //    {
    //        SettingsKey = settingsKey;
    //    }
        
    //}
}

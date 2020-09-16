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

        internal void Save()
        {
            UserSettings.Default.Save();
        }
    }

    internal partial class UserSettings
    {
        internal UserSettings()
        {
            General.PropertyChanged += Settings_PropertyChanged;
            AutoSave = true;
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (AutoSave)
                Save();
        }

        internal static GeneralUserSettings General { get; } = new GeneralUserSettings();

        internal static bool AutoSave { get; set; }
    }

    public class GeneralUserSettings : UserSettingsBase
    {
        public bool WarnDeleteSource
        {
            get { return UserSettings.Default.WarnDeleteSource; }
            set
            {
                UserSettings.Default.WarnDeleteSource = value;
                RaisePropertyChanged(nameof(WarnDeleteSource));
            }
        }

        public bool ShowPreviewWindow
        {
            get { return UserSettings.Default.ShowPreviewWindow; }
            set
            {
                UserSettings.Default.ShowPreviewWindow = value;
                RaisePropertyChanged(nameof(ShowPreviewWindow));
            }
        }

        public bool ShowSummaryWindow
        {
            get { return UserSettings.Default.ShowSummaryWindow; }
            set
            {
                UserSettings.Default.ShowSummaryWindow = value;
                RaisePropertyChanged(nameof(ShowSummaryWindow));
            }
        }

        public bool BackgroundReading
        {
            get { return UserSettings.Default.BackgroundReading; }
            set
            {
                UserSettings.Default.BackgroundReading = value;
                RaisePropertyChanged(nameof(BackgroundReading));
            }
        }
    }

}

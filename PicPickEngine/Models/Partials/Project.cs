using PicPick.Core;
using PicPick.Models.IsDirtySupport;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using PicPick.Models.Interfaces;

namespace PicPick.Models
{
    public delegate void CopyEventHandler(object sender, CopyEventArgs e);

    public partial class PicPickProject : IIsDirtySupport, IProject
    {
        private ObservableCollection<IActivity> _activityList = null;
        private string _name;
        private IsDirtySupport<IIsDirtySupport> _isDirtySupport;

        #region IsDirty

        public IsDirtySupport<IIsDirtySupport> GetIsDirtyInstance()
        {
            if (_isDirtySupport == null)
                _isDirtySupport = new IsDirtySupport<IIsDirtySupport>(this);
            return _isDirtySupport;
        }
        
        
        /// <summary>
        /// Wrapping IsDirtySupport class property
        /// </summary>
        [XmlIgnore]
        public bool IsDirty
        {
            get { return _isDirtySupport?.IsDirty ?? false;  }
            set { if (_isDirtySupport != null) _isDirtySupport.IsDirty = value; }
        }

        #endregion


        // Use this list rather than the Activity Array for easyer manipulations and editing.
        // This will be converted back to the Activity Array in Loader.Save()
        [XmlIgnore]
        public ObservableCollection<IActivity> ActivityList
        {
            get
            {
                if (_activityList == null)
                {
                    _activityList = new ObservableCollection<IActivity>();
                    if (this.Activities != null)
                        foreach (IActivity activity in this.Activities)
                        {
                            _activityList.Add(activity);
                        }
                }
                return _activityList;
            }
        }
        
        [XmlIgnore]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                this.RaisePropertyChanged("Name");
            }
        }

        [XmlIgnore]
        [IsDirtyIgnore]
        public PicPickProject_options Options
        {
            get
            {
                if (_options == null) _options = new PicPickProject_options();
                return _options;
            }
            set
            {
                this._options = value;
            }
        }


    }


}

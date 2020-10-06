using PicPick.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TalUtils;
using System.Collections.ObjectModel;
using PicPick.Core;
using PicPick.Models.Interfaces;
using log4net;
using PicPick.Models.Mapping;
using System.Threading;
using PicPick.StateMachine;
using PicPick.Exceptions;
using System.IO;

namespace PicPick.Models
{
    /// <summary>
    /// Need refactoring!
    /// Currently used as an indication for IsRunning across view models.
    /// </summary>
    public enum ActivityState
    {
        NOT_STARTED,
        RUNNING,
        DONE
    }

    public delegate void ActivityStateChangedEventHandler(PicPickProjectActivity activity, ActivityStateChangedEventArgs e);

    /// <summary>
    /// This class extends PicPickProjectActivity and includes:
    /// 1. Extension properties
    /// 2. Execution methods
    /// </summary>
    public partial class PicPickProjectActivity : ICloneable, IActivity
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly ErrorHandler _errorHandler = new ErrorHandler(_log);

        private ObservableCollection<PicPickProjectActivityDestination> _destinationList = null;
        private StateManager _stateMachine;
        private ActivityState _state;

        public event ActivityStateChangedEventHandler OnActivityStateChanged;



        public static PicPickProjectActivity CreateNew(string name)
        {
            PicPickProjectActivity activity = new PicPickProjectActivity()
            {
                Name = name
            };

            activity.Source = PicPickProjectActivitySource.CreateNew();
            activity.DestinationList.Add(PicPickProjectActivityDestination.CreateNew(activity));

            return activity;
        }



        public void ValidateFields()
        {
            string realPath;
            bool isRealTemplate;

            if (Source == null || Source.Path == "")
                throw new NoSourceException();

            if (!Directory.Exists(Source.Path))
                throw new SourceDirectoryNotFoundException();

            if (DestinationList.Where(d => d.Active).Count() == 0)
                throw new NoDestinationsException();

            foreach (var dest in DestinationList.Where(d => d.Active))
            {
                realPath = dest.GetTemplatePath(DateTime.Now);
                isRealTemplate = !realPath.Equals(dest.Template);
                if (isRealTemplate)
                    continue;

                realPath = Path.Combine(PathHelper.GetFullPath(Source.Path, dest.Path), dest.Template);

                if (realPath.Equals(Source.Path, StringComparison.OrdinalIgnoreCase))
                    throw new DestinationEqualsSourceException();
            }
        }


        /// <summary>
        /// Use this list rather than the Destination Array for easyer manipulations and editing.
        /// This will be converted back to the Destination Array in ConfigurationHelper.Save()
        /// </summary>
        [XmlIgnore]
        public ObservableCollection<PicPickProjectActivityDestination> DestinationList
        {
            get
            {
                if (_destinationList == null)
                {
                    if (this.Destination == null)
                        this.Destination = new PicPickProjectActivityDestination[0];
                    _destinationList = new ObservableCollection<PicPickProjectActivityDestination>();

                    foreach (PicPickProjectActivityDestination dest in this.Destination)
                    {
                        _destinationList.Add(dest);
                    }
                }
                return _destinationList;
            }
            set
            {
                _destinationList = value;
            }
        }


        [XmlIgnore]
        public FileExistsResponseEnum FileExistsResponse { get; set; }

        [XmlIgnore]
        [IsDirtySupport.IsDirtyIgnore]
        public bool IsRunning
        {
            get => State == ActivityState.RUNNING;
            set => State = value ? ActivityState.RUNNING : ActivityState.NOT_STARTED;
        }

        [XmlIgnore]
        [IsDirtySupport.IsDirtyIgnore]
        public ActivityState State
        {
            get => _state; set
            {
                _state = value;
                RaisePropertyChanged(nameof(State));
                OnActivityStateChanged?.Invoke(this, new ActivityStateChangedEventArgs());
            }
        }

        [XmlIgnore]
        [IsDirtySupport.IsDirtyIgnore]
        public StateManager StateMachine
        {
            get
            {
                if (_stateMachine == null) _stateMachine = new StateManager(this);
                return _stateMachine;
            }
            set => _stateMachine = value;
        }

        [XmlIgnore]
        public FilesGraph FileGraph { get; set; }


        #region ICloneable

        public object Clone()
        {
            PicPickProjectActivity newActivity = (PicPickProjectActivity)this.MemberwiseClone();
            if (Source != null)
                newActivity.Source = (PicPickProjectActivitySource)this.Source.Clone();

            newActivity.DestinationList = new ObservableCollection<PicPickProjectActivityDestination>(DestinationList.Select(dst => (PicPickProjectActivityDestination)dst.Clone()).ToList());

            // this will force StateMachine recreation
            newActivity.StateMachine = null;
            newActivity.IsRunning = false;

            return newActivity;
        }

        public PicPickProjectActivity Clone(string newName)
        {
            PicPickProjectActivity newActivity = (PicPickProjectActivity)Clone();
            newActivity.Name = newName;
            return newActivity;
        }

        #endregion
    }

}

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

namespace PicPick.Models
{

    public enum ACTIVITY_STATE
    {
        NOT_STARTED,
        ANALYZING,
        ANALYZED,
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
        private bool _isRunning;
        private ActivityFileMapping _fileMapping;
        private Runner _runner;

        public event ActivityStateChangedEventHandler OnActivityStateChanged;

        public PicPickProjectActivity(string name)
        {
            Name = name;
            Source = new PicPickProjectActivitySource();
            Source.FromDate = new DateComplex();
            Source.FromDate.Date = DateTime.Today.AddDays(-30);
            Source.ToDate = new DateComplex();
            Source.ToDate.Date = DateTime.Today;
        }

        private bool SetState(ACTIVITY_STATE newState)
        {
            this.State = newState;
            if (OnActivityStateChanged != null)
            {
                ActivityStateChangedEventArgs e = new ActivityStateChangedEventArgs();
                OnActivityStateChanged(this, e);
                if (e.Cancel)
                    return false;
            }
            return true;
        }

        public async Task ExecuteAsync(ProgressInformation progressInfo)
        {
            if (_isRunning) throw new Exception("Activity is already running.");

            try
            {
                _isRunning = true;

                // Analyze
                SetState(ACTIVITY_STATE.ANALYZING);
                await FileMapping.ComputeAsync(progressInfo);
                if (!SetState(ACTIVITY_STATE.ANALYZED) || RunMode_AnalyzeOnly)
                    return;

                // Run
                SetState(ACTIVITY_STATE.RUNNING);
                await Runner.Run(progressInfo);
                SetState(ACTIVITY_STATE.DONE);
            }
            catch (OperationCanceledException)
            {
                _log.Info("*** The user cancelled the operation ***");
                progressInfo.UserCancelled = true;
            }
            catch (Exception ex)
            {
                progressInfo.Exception = ex;
                _errorHandler.Handle(ex, false, $"Error in operation: {progressInfo.Header}.");
            }
            finally
            {
                string analyzeOnlyMode = RunMode_AnalyzeOnly ? " (AnalyzeOnly mode)" : "";
                _log.Info($"Finished{analyzeOnlyMode}: {Name}\n*****************");
                progressInfo.Finished();
                if (!RunMode_AnalyzeOnly)
                    EventAggregatorHelper.PublishActivityEnded();
                _isRunning = false;
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
        public bool Initialized { get; set; }

        [XmlIgnore]
        public FileExistsResponseEnum FileExistsResponse { get; set; }

        [XmlIgnore]
        [IsDirtySupport.IsDirtyIgnore]
        public ActivityFileMapping FileMapping
        {
            get
            {
                if (_fileMapping == null)
                    _fileMapping = new ActivityFileMapping(this);
                return _fileMapping;
            }
            set => _fileMapping = value;
        }

        [XmlIgnore]
        [IsDirtySupport.IsDirtyIgnore]
        public Runner Runner
        {
            get
            {
                if (_runner == null)
                    _runner = new Runner(this, ProjectLoader.Project.Options);
                return _runner;
            }
            set => _runner = value;
        }

        [XmlIgnore]
        [IsDirtySupport.IsDirtyIgnore]
        public ACTIVITY_STATE State { get; set; }

        [XmlIgnore]
        [IsDirtySupport.IsDirtyIgnore]
        public bool RunMode_AnalyzeOnly { get; set; }


        #region ICloneable

        public object Clone()
        {
            PicPickProjectActivity newActivity = (PicPickProjectActivity)this.MemberwiseClone();
            if (Source != null)
                newActivity.Source = (PicPickProjectActivitySource)this.Source.Clone();

            newActivity.DestinationList = new ObservableCollection<PicPickProjectActivityDestination>(DestinationList.Select(dst => (PicPickProjectActivityDestination)dst.Clone()).ToList());

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

using log4net;
using PicPick.Core;
using PicPick.Helpers;
using PicPick.Models;
using PicPick.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TalUtils;

namespace PicPick.StateMachine
{
    public delegate void StateChangedEventHandler(object sender, StateChangedEventArgs e);

    public class StateChangedEventArgs : CancellableEventArgs
    { }

    public enum PicPickState
    {
        READY,
        READING,
        MAPPING,
        FILTERING,
        READY_TO_RUN,
        RUNNING,
        DONE
    }

    public class StateManager
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly ErrorHandler _errorHandler = new ErrorHandler(_log);

        public event StateChangedEventHandler OnStateChanged;


        public PicPickState EndState { get; set; }

        public IActivity Activity { get; private set; }
        public IProgressInformation ProgressInfo { get; set; }
        public CoreActions CoreActions { get; private set; }

        private Dictionary<PicPickState, IStateHandler> _stateTransitions = new Dictionary<PicPickState, IStateHandler>();
        private PicPickState _currentState;
        private PicPickState? _needRestartFromState;

        object lockNeedRestart = new object();

        public StateManager(IActivity activity)
        {
            Activity = activity;
            Activity.FileGraph = new FilesGraph();
            CoreActions = new CoreActions(Activity);

            _stateTransitions.Add(PicPickState.READING, new StateTransition_Read(this));
            _stateTransitions.Add(PicPickState.MAPPING, new StateTransition_Map(this));
            _stateTransitions.Add(PicPickState.FILTERING, new StateTransition_Filter(this));
            _stateTransitions.Add(PicPickState.RUNNING, new StateTransition_Run(this));

            
        }

        public StateManager(IActivity activity, IProgressInformation progressInfo) : this(activity)
        {
            ProgressInfo = progressInfo;
        }

        public void Start(PicPickState toState)
        {
            if (!Enabled)
            {
                SetNeedRestart(CurrentState);
                return;
            }

            EndState = toState;
            _ = StartAsync();
        }

        private async Task StartAsync()
        {
            bool stateResult = true;

            if (IsRunning)
                return;

            IsRunning = true;
            LastException = null;

            if (CurrentState == PicPickState.DONE)
                CurrentState = PicPickState.READY;

            lock (lockNeedRestart)
            {
                if (_needRestartFromState.HasValue)
                {
                    if (_needRestartFromState < CurrentState)
                        CurrentState = _needRestartFromState.Value;
                    NeedRestartValue = null;
                }
            }

            try
            {
                while (CurrentState < EndState)
                {
                    ProgressInfo.Reset();
                    if (_stateTransitions.ContainsKey(CurrentState))
                    {
                        try
                        {
                            stateResult = await _stateTransitions[CurrentState].ExecuteAsync();
                        }
                        catch (OperationCanceledException)
                        {
                            lock (this)
                            {
                                if (ProgressInfo.OperationCancelled)
                                    break;
                                continue;
                            }
                        }
                    }

                    lock (this)
                    {
                        if (!stateResult || ProgressInfo.OperationCancelled)
                            break;
                        if (!PublishStateChangedEvent())
                            break;
                        CurrentState = GetNextState(CurrentState);
                    }
                };
            }
            catch (Exception ex)
            {
                // IMPORTANT: we should get here only if something is wrong with the paramters.
                // Errors that could be handled by an external change and re-run should be handled differently!
                LastException = ex;
                _errorHandler.Handle(ex);
                EventAggregatorHelper.PublishGeneralException(new ExceptionEventArgs(ex));
            }
            finally
            {
                if (ProgressInfo.OperationCancelled)
                    CurrentState = GetStoppingState();
                IsRunning = false;
                PublishStateChangedEvent();
            }

            if (CurrentState == PicPickState.DONE)
            {
                CurrentState = PicPickState.READY;
                PublishStateChangedEvent();
            }
        }

        private bool PublishStateChangedEvent()
        {
            if (OnStateChanged == null)
                return true;

            StateChangedEventArgs e = new StateChangedEventArgs();
            OnStateChanged.Invoke(this, e);
            return !e.Cancel;
        }

        private PicPickState GetStoppingState()
        {
            return CurrentState > PicPickState.READY_TO_RUN ? PicPickState.DONE : PicPickState.READY;
        }

        /// <summary>
        /// Restarts the process from the given state ONLY if we already passed that state.
        /// </summary>
        /// <param name="fromtState"></param>
        public void Restart(PicPickState fromtState, PicPickState toState)
        {
            lock (this)
            {
                Stop(false);
                if (CurrentState > fromtState)
                    CurrentState = fromtState;
            }

            Start(toState);
        }

        public void Stop(bool cancelOperation = true)
        {
            lock (this)
            {
                if (IsRunning)
                {
                    if (cancelOperation)
                        ProgressInfo.OperationCancelled = true;
                    ProgressInfo.Cancel();
                }
            }
        }

        public bool IsRunning { get; private set; }


        private PicPickState GetNextState(PicPickState state)
        {
            if (state == PicPickState.DONE)
                return PicPickState.READY;

            return (PicPickState)((int)state + 1);
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        public PicPickState CurrentState
        {
            get => _currentState;
            private set
            {
                _currentState = value;
                RaisePropertyChanged(nameof(CurrentState));
            }
        }

        private PicPickState? NeedRestartValue 
        {
            get
            {
                return _needRestartFromState;
            }
            set
            {
                _needRestartFromState = value;
                RaisePropertyChanged(nameof(NeedRestartValue));
                RaisePropertyChanged(nameof(NeedRestart));
            }
        }

        public void SetNeedRestart(PicPickState needRestartFromState)
        {
            lock (lockNeedRestart)
            {
                if (!_needRestartFromState.HasValue || _needRestartFromState.Value > needRestartFromState)
                    NeedRestartValue = needRestartFromState;
            }
        }

        public bool NeedRestart
        {
            get
            {
                return _needRestartFromState.HasValue;
            }
        }

        // Note that this exception is preventing the process from running until something is changed
        public Exception LastException { get; private set; }

        public static bool Enabled { get; set; }
    }
}

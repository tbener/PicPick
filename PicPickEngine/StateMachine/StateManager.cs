using PicPick.Core;
using PicPick.Helpers;
using PicPick.Models;
using PicPick.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public event StateChangedEventHandler OnStateChanged;


        public PicPickState EndState { get; set; }

        public IActivity Activity { get; private set; }
        public IProgressInformation ProgressInfo { get; set; }
        public CoreActions CoreActions { get; private set; }

        private Dictionary<PicPickState, IStateHandler> _stateTransitions = new Dictionary<PicPickState, IStateHandler>();
        private PicPickState _currentState;

        public StateManager(IActivity activity)
        {
            Activity = activity;
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
            EndState = toState;
            _ = StartAsync();
        }

        private async Task StartAsync()
        {
            if (IsRunning)
                return;

            IsRunning = true;

            if (CurrentState == PicPickState.DONE)
                CurrentState = PicPickState.READY;

            try
            {
                while (CurrentState < EndState)
                {
                    ProgressInfo.Reset();
                    if (_stateTransitions.ContainsKey(CurrentState))
                    {
                        try
                        {
                            await _stateTransitions[CurrentState].ExecuteAsync();
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
                        if (ProgressInfo.OperationCancelled)
                            break;
                        if (!PublishStateChangedEvent())
                            break;
                        CurrentState = GetNextState(CurrentState);
                    }
                };
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

        public void Stop(PicPickState? setState = null)
        {
            lock (this)
            {
                if (IsRunning)
                {
                    Stop(true);
                    //if (setState.HasValue)
                    //    CurrentState = setState.Value;
                    //else
                    //{
                    //    if (CurrentState < PicPickState.READY_TO_RUN)
                    //        CurrentState = PicPickState.READY;
                    //    else
                    //        CurrentState = PicPickState.READY_TO_RUN;

                    //}
                }
                else
                {
                    //if (setState.HasValue)
                    //    CurrentState = setState.Value;
                }
            }
        }

        private void Stop(bool cancelOperation)
        {
            if (IsRunning)
            {
                if (cancelOperation)
                    ProgressInfo.OperationCancelled = true;
                ProgressInfo.Cancel();
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

    }
}

using PicPick.Core;
using PicPick.Helpers;
using PicPick.Models;
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

        public PicPickState CurrentState
        {
            get => currentState;
            private set
            {
                currentState = value;
                RaisePropertyChanged(nameof(CurrentState));
            }
        }
        public PicPickState EndState { get; set; }

        public PicPickProjectActivity Activity { get; private set; }
        public ProgressInformation ProgressInfo { get; set; }
        public CoreActions CoreActions { get; private set; }

        private Dictionary<PicPickState, IStateHandler> _stateTransitions = new Dictionary<PicPickState, IStateHandler>();
        private PicPickState currentState;

        public StateManager(PicPickProjectActivity activity)
        {
            Activity = activity;
            CoreActions = new CoreActions(Activity);

            _stateTransitions.Add(PicPickState.READING, new StateTransition_Read(this));
            _stateTransitions.Add(PicPickState.MAPPING, new StateTransition_Map(this));
            _stateTransitions.Add(PicPickState.FILTERING, new StateTransition_Filter(this));
            _stateTransitions.Add(PicPickState.RUNNING, new StateTransition_Run(this));
        }

        public StateManager(PicPickProjectActivity activity, ProgressInformation progressInfo) : this(activity)
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

            try
            {
                while (CurrentState < EndState)
                {
                    if (_stateTransitions.ContainsKey(CurrentState))
                    {
                        ProgressInfo.Reset();
                        try
                        {
                            await _stateTransitions[CurrentState].ExecuteAsync();
                        }
                        catch (OperationCanceledException)
                        {
                            if (ProgressInfo.OperationCancelled) return;
                            continue;
                        }
                    }
                    if (!PublishStateChangedEvent())
                        return;
                    CurrentState = GetNextState(CurrentState);
                };
            }
            finally
            {
                IsRunning = false;
                PublishStateChangedEvent();
                ProgressInfo.Reset();
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

        /// <summary>
        /// Restarts the process from the given state ONLY if we already passed that state.
        /// </summary>
        /// <param name="fromtState"></param>
        public void Restart(PicPickState fromtState, PicPickState toState)
        {
            if (CurrentState < fromtState)
                return;

            if (IsRunning)
                ProgressInfo.Cancel();

            CurrentState = fromtState;
            Start(toState);
        }

        public void Stop()
        {
            ProgressInfo.OperationCancelled = true;
            if (IsRunning)
                ProgressInfo.Cancel();
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

    }
}

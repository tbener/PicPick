using PicPick.Core;
using PicPick.Helpers;
using PicPick.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.StateMachine
{
    public enum PicPickState
    {
        NOT_STARTED,
        READ,
        MAP,
        FILTER,
        READY_TO_RUN,
        RUN,
        DONE
    }

    public class StateManager
    {
        public event EventHandler OnStateChanged;

        private bool _isRunning = false;

        public PicPickState CurrentState { get; private set; }
        public PicPickState EndState { get; set; }
        public PicPickProjectActivity Activity { get; private set; }
        public ProgressInformation ProgressInfo { get; set; }
        public CoreActions CoreActions { get; private set; }

        private Dictionary<PicPickState, IStateHandler> _stateTransitions = new Dictionary<PicPickState, IStateHandler>();

        public StateManager(PicPickProjectActivity activity)
        {
            Activity = activity;
            CoreActions = new CoreActions(Activity);

            _stateTransitions.Add(PicPickState.READ, new StateTransition_Read(this));
            _stateTransitions.Add(PicPickState.MAP, new StateTransition_Map(this));
            _stateTransitions.Add(PicPickState.FILTER, new StateTransition_Filter(this));
            _stateTransitions.Add(PicPickState.RUN, new StateTransition_Run(this));
        }

        public StateManager(PicPickProjectActivity activity, ProgressInformation progressInfo) : this(activity)
        {
            ProgressInfo = progressInfo;
        }

        public void Start(PicPickState startState, PicPickState endState)
        {
            CurrentState = startState;
            EndState = endState;
            Start();
        }

        public void Start()
        {
            Task.Run(() => StartAsync());
        }

        public async Task StartAsync()
        {
            _isRunning = true;

            try
            {
                while (CurrentState <= EndState)
                {
                    if (_stateTransitions.ContainsKey(CurrentState))
                        await _stateTransitions[CurrentState].ExecuteAsync();
                    OnStateChanged?.Invoke(this, null);

                    CurrentState = GetNextState(CurrentState);
                };
            }
            finally
            {
                _isRunning = false;
                OnStateChanged?.Invoke(this, null);
            }

        }

        public void Run()
        {
            if (CurrentState > PicPickState.RUN)
                CurrentState = PicPickState.RUN;

            ContinueTo(PicPickState.DONE);
        }

        public void ContinueTo(PicPickState endState)
        {
            EndState = endState;

            if (_isRunning)
                return;

            Start();
        }

        public void StartFrom(PicPickState startState)
        {
            if (_isRunning)
                Stop();

            CurrentState = startState;
            Start();

        }

        /// <summary>
        /// Restarts the process from the given state ONLY if we already passed that state.
        /// </summary>
        /// <param name="startState"></param>
        public void RestartFrom(PicPickState startState)
        {
            if (CurrentState < startState)
                return;

            if (_isRunning)
                Stop();

            CurrentState = startState;
            Start();

        }

        public void Stop()
        {
            ProgressInfo.Cancel();
        }

        private PicPickState GetNextState(PicPickState state)
        {
            switch (state)
            {
                case PicPickState.NOT_STARTED:
                    return PicPickState.READ;
                case PicPickState.READ:
                    return PicPickState.MAP;
                case PicPickState.MAP:
                    return PicPickState.FILTER;
                case PicPickState.FILTER:
                    return PicPickState.READY_TO_RUN;
                case PicPickState.READY_TO_RUN:
                    return PicPickState.RUN;
                case PicPickState.RUN:
                    return PicPickState.DONE;
                default:
                    return PicPickState.NOT_STARTED;
            }
        }

        private PicPickState GetPreviousState(PicPickState state)
        {
            switch (state)
            {
                case PicPickState.READ:
                    return PicPickState.NOT_STARTED;
                case PicPickState.MAP:
                    return PicPickState.READ;
                case PicPickState.FILTER:
                    return PicPickState.MAP;
                case PicPickState.RUN:
                    return PicPickState.FILTER;
                case PicPickState.DONE:
                    return PicPickState.RUN;
                default:
                    return PicPickState.DONE;
            }
        }

        public string GetStatus()
        {
            switch (CurrentState)
            {
                case PicPickState.NOT_STARTED:
                    return "Not started";
                case PicPickState.READ:
                    return _isRunning ? "Reading..." : "Not mapped";
                case PicPickState.MAP:
                    return _isRunning ? "Mapping..." : "Not filtered";
                case PicPickState.FILTER:
                    return _isRunning ? "Filtering..." : "Ready";
                case PicPickState.READY_TO_RUN:
                    return "Ready";
                case PicPickState.RUN:
                    return "Running...";
                case PicPickState.DONE:
                    return "Done";
                default:
                    return "Unknown";
            }
        }
    }
}

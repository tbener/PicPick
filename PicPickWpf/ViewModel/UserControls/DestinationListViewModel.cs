using PicPick.Commands;
using PicPick.Helpers;
using PicPick.Models;
using PicPick.Models.Interfaces;
using PicPick.StateMachine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PicPick.ViewModel.UserControls
{
    public class DestinationListViewModel : ActivityBaseViewModel
    {
        #region Private Members

        private bool _keepDestinationsAbsolute;
        private bool _enabled = true;

        #endregion

        #region Commands

        public ICommand AddDestinationCommand { get; set; }

        #endregion

        #region CTOR

        public DestinationListViewModel(IActivity activity, IProgressInformation progressInfo) : base(activity, progressInfo)
        {
            Activity.OnActivityStateChanged += (s, e) =>
            {
                Enabled = !IsRunning;
            };

            AddDestinationCommand = new RelayCommand(AddDestination);

            DestinationViewModelList = new ObservableCollection<DestinationViewModel>();
            foreach (PicPickProjectActivityDestination dest in Activity.DestinationList)
                AddDestinationViewModel(dest);
        }

        #endregion

        #region Private Methods

        private void OnDestinationDelete(object sender, EventArgs e)
        {
            DestinationViewModel vm = sender as DestinationViewModel;
            if (vm == null)
                return;
            if (vm.Destination != null)
            {
                vm.Destination.PropertyChanged -= Destination_PropertyChanged;
                Activity.DestinationList.Remove(vm.Destination);
            }
            DestinationViewModelList.Remove(vm);
            if (Activity.DestinationList.Count == 1)
                Activity.DestinationList.First().Active = true;
            UpdateMapping();
        }

        private void AddDestination()
        {
            PicPickProjectActivityDestination dest = new PicPickProjectActivityDestination()
            {
                Path = "",
                Template = "dd-yy"
            };
            Activity.DestinationList.Add(dest);
            AddDestinationViewModel(dest);
        }

        private void AddDestinationViewModel(PicPickProjectActivityDestination dest)
        {
            var vm = new DestinationViewModel(dest, Activity.Source);
            vm.OnDeleteClicked += OnDestinationDelete;
            DestinationViewModelList.Add(vm);
            UpdateMapping();
            dest.PropertyChanged += Destination_PropertyChanged;
        }

        private void Destination_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateMapping();
        }

        private void UpdateMapping()
        {
            if (!BackgroundReadingEnabled) 
                return;

            Activity.StateMachine.Restart(PicPickState.MAPPING, BACKGROUND_END_STATE);
        }

        #endregion

        #region Public Properties

        public ObservableCollection<DestinationViewModel> DestinationViewModelList { get; set; }

        public bool KeepDestinationsAbsolute
        {
            get { return DestinationViewModelList.FirstOrDefault().Destination.KeepAbsolute; }
            set
            {
                _keepDestinationsAbsolute = value;
                foreach (DestinationViewModel destvm in DestinationViewModelList)
                {
                    destvm.Destination.KeepAbsolute = _keepDestinationsAbsolute;
                }
            }
        }

        public bool Enabled
        {
            get => _enabled;
            internal set
            {
                _enabled = value;
                OnPropertyChanged(nameof(Enabled));
            }
        }


        #endregion

        #region IDisposable Implementation

        public override void Dispose()
        {
            DestinationViewModelList.Clear();
            DestinationViewModelList = null;

            base.Dispose();
        }

        #endregion
    }
}

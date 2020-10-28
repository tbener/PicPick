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
        #region Commands

        public ICommand AddDestinationCommand { get; set; }

        #endregion

        #region CTOR

        public DestinationListViewModel(IActivity activity, IProgressInformation progressInfo) : base(activity, progressInfo)
        {
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
            UpdateMapping(PicPickState.MAPPING);
        }

        private void AddDestination()
        {
            PicPickProjectActivityDestination dest = new PicPickProjectActivityDestination(Activity)
            {
                Path = "",
                Template = "dd-yy"
            };
            Activity.DestinationList.Add(dest);
            AddDestinationViewModel(dest);
            UpdateMapping(PicPickState.MAPPING);
        }

        private void AddDestinationViewModel(PicPickProjectActivityDestination dest)
        {
            var vm = new DestinationViewModel(dest, Activity.Source);
            vm.OnDeleteClicked += OnDestinationDelete;
            DestinationViewModelList.Add(vm);
            dest.PropertyChanged += Destination_PropertyChanged;
        }

        private void Destination_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateMapping(PicPickState.MAPPING);
        }

        

        #endregion

        #region Public Properties

        public ObservableCollection<DestinationViewModel> DestinationViewModelList { get; set; }

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

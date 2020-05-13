using PicPick.Commands;
using PicPick.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PicPick.ViewModel.UserControls
{
    public class DestinationListViewModel : BaseViewModel, IDisposable
    {
        #region Private Members

        private bool _keepDestinationsAbsolute;

        #endregion        
        
        #region Commands

        public ICommand AddDestinationCommand { get; set; }

        #endregion

        #region CTOR

        public DestinationListViewModel(PicPickProjectActivity activity)
        {
            Activity = activity;

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
                Activity.DestinationList.Remove(vm.Destination);
            DestinationViewModelList.Remove(vm);
            if (Activity.DestinationList.Count == 1)
                Activity.DestinationList.First().Active = true;
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
        }

        #endregion

        #region Public Properties

        public PicPickProjectActivity Activity { get; set; }
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

        #endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            Activity = null;
            DestinationViewModelList.Clear();
            DestinationViewModelList = null;
        }

        #endregion
    }
}

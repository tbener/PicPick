using System.Windows;
using PicPick.Helpers;
using PicPick.StateMachine;
using PicPick.Models.Interfaces;

namespace PicPick.ViewModel.UserControls
{
    public class ActivityViewModel : ActivityBaseViewModel
    {
        

        #region CTOR

        public ActivityViewModel(IActivity activity, IProgressInformation progressInfo) : base(activity, progressInfo)
        {
            Activity.StateMachine.ProgressInfo = ProgressInfo;

            SourceViewModel = new SourceViewModel(activity, progressInfo);
            DestinationListViewModel = new DestinationListViewModel(activity, progressInfo);
            ExecutionViewModel = new ExecutionViewModel(activity, ProgressInfo);

            Activity.StateMachine.Start(BACKGROUND_END_STATE);
        }

        #endregion

        public override bool BackgroundReadingEnabled
        {
            get => base.BackgroundReadingEnabled;
            set
            {
                base.BackgroundReadingEnabled = value;
                if (base.BackgroundReadingEnabled)
                    Activity.StateMachine.Restart(PicPickState.READING, BACKGROUND_END_STATE);
                else
                    Activity.StateMachine.Stop();
            }
        }


        #region Activity parts view models

        public SourceViewModel SourceViewModel
        {
            get { return (SourceViewModel)GetValue(SourceViewModelProperty); }
            set { SetValue(SourceViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SourceViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceViewModelProperty =
            DependencyProperty.Register("SourceViewModel", typeof(SourceViewModel), typeof(ActivityViewModel), new PropertyMetadata(null));


        public DestinationListViewModel DestinationListViewModel
        {
            get { return (DestinationListViewModel)GetValue(DestinationListViewModelProperty); }
            set { SetValue(DestinationListViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DestinationListViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DestinationListViewModelProperty =
            DependencyProperty.Register("DestinationListViewModel", typeof(DestinationListViewModel), typeof(ActivityViewModel), new PropertyMetadata(null));


        public ExecutionViewModel ExecutionViewModel
        {
            get { return (ExecutionViewModel)GetValue(ExecutionViewModelProperty); }
            set { SetValue(ExecutionViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExecutionViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExecutionViewModelProperty =
            DependencyProperty.Register("ExecutionViewModel", typeof(ExecutionViewModel), typeof(ActivityViewModel), new PropertyMetadata(null));


        #endregion


    }
}

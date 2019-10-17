using Prism.Events;

namespace PicPick.Helpers
{
    internal class ApplicationService
    {
        private ApplicationService() { }

        internal static ApplicationService Instance { get; } = new ApplicationService();

        private IEventAggregator _eventAggregator;
        internal IEventAggregator EventAggregator
        {
            get
            {
                if (_eventAggregator == null)
                    _eventAggregator = new EventAggregator();

                return _eventAggregator;
            }
        }
    }
}

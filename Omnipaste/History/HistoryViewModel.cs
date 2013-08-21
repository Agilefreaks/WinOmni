namespace Omnipaste.History
{
    using System.Collections.ObjectModel;
    using Caliburn.Micro;
    using OmniCommon.Interfaces;

    public class HistoryViewModel : Screen, IHistoryViewModel
    {
        private ObservableCollection<string> _clippings;

        public ObservableCollection<string> Clippings
        {
            get
            {
                return _clippings;
            }

            set
            {
                _clippings = value;
                NotifyOfPropertyChange(() => Clippings);
            }
        }

        public HistoryViewModel(IEventAggregator eventAggregator)
        {
            _clippings = new ObservableCollection<string>();
            eventAggregator.Subscribe(this);
        }

        public void Handle(IClipboardData message)
        {
            while (_clippings.Count > 4)
            {
                _clippings.RemoveAt(4);
            }

            Clippings.Insert(0, message.GetData());
        }
    }
}

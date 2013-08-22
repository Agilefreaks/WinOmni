namespace Omnipaste.History
{
    using Caliburn.Micro;
    using OmniCommon.Domain;
    using OmniCommon.Interfaces;
    using System.Linq;
    using System.Collections.ObjectModel;

    public class HistoryViewModel : Screen, IHistoryViewModel
    {
        private Clipping _selectedClipping;
        private ObservableCollection<Clipping> _clippings;

        public ObservableCollection<Clipping> RecentClippings
        {
            get
            {
                var last5Clippings = Clippings != null ? Clippings.Take(5) : Enumerable.Empty<Clipping>();
                return new ObservableCollection<Clipping>(last5Clippings);
            }
        }

        public ObservableCollection<Clipping> Clippings
        {
            get
            {
                return _clippings;
            }

            set
            {
                _clippings = value;
                NotifyOfPropertyChange(() => Clippings);
                NotifyOfPropertyChange(() => HasClippings);
            }
        }

        public bool HasClippings
        {
            get { return RecentClippings != null && RecentClippings.Any(); }
        }

        public Clipping SelectedClipping
        {
            get
            {
                return _selectedClipping;
            }

            set
            {
                _selectedClipping = value;
                NotifyOfPropertyChange(() => SelectedClipping);
            }
        }

        public IOmniService OmniService { get; set; }

        public HistoryViewModel(IOmniService omniService, IEventAggregator eventAggregator)
        {
            OmniService = omniService;
            Clippings = new ObservableCollection<Clipping>(OmniService.GetClippings());
            eventAggregator.Subscribe(this);
        }

        public void Handle(IClipboardData message)
        {
            Clippings = new ObservableCollection<Clipping>(OmniService.GetClippings());

            NotifyOfPropertyChange(() => HasClippings);
            NotifyOfPropertyChange(() => RecentClippings);
        }

        public void SetSelectedClipping()
        {
            if (SelectedClipping != null)
            {
                OmniService.LocalClipboard.PutData(SelectedClipping.Content);
            }
        }
    }
}

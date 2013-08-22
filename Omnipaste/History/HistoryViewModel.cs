namespace Omnipaste.History
{
    using Caliburn.Micro;
    using OmniCommon.Domain;
    using OmniCommon.Interfaces;
    using System.Linq;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;

    public class HistoryViewModel : Screen, IHistoryViewModel
    {
        private ObservableCollection<string> _recentClippings;
        private Clipping _selectedClipping;
        private ObservableCollection<Clipping> _clippings;

        public ObservableCollection<string> RecentClippings
        {
            get
            {
                return _recentClippings;
            }

            set
            {
                if (_recentClippings != null)
                {
                    _recentClippings.CollectionChanged -= OnRecentClippingsChanged;
                }

                _recentClippings = value;

                if (_recentClippings != null)
                {
                    _recentClippings.CollectionChanged += OnRecentClippingsChanged;
                }

                NotifyOfPropertyChange(() => Clippings);
                NotifyOfPropertyChange(() => HasClippings);
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
            RecentClippings = new ObservableCollection<string>();
            eventAggregator.Subscribe(this);
        }

        public void Handle(IClipboardData message)
        {
            while (_recentClippings.Count > 4)
            {
                _recentClippings.RemoveAt(4);
            }

            RecentClippings.Insert(0, message.GetData());
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            Clippings = new ObservableCollection<Clipping>(OmniService.GetClippings());
        }

        public void OnRecentClippingsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Clippings = new ObservableCollection<Clipping>(OmniService.GetClippings());

            NotifyOfPropertyChange(() => HasClippings);
        }
    }
}

using System.Collections.Specialized;
using System.Linq;
using OmniCommon.Domain;

namespace Omnipaste.History
{
    using System.Collections.ObjectModel;
    using Caliburn.Micro;
    using OmniCommon.Interfaces;

    public class HistoryViewModel : Screen, IHistoryViewModel
    {
        public IClippingRepository ClippingRepository { get; set; }
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

        public HistoryViewModel(IClippingRepository clippingRepository, IEventAggregator eventAggregator)
        {
            ClippingRepository = clippingRepository;
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

            Clippings = new ObservableCollection<Clipping>(ClippingRepository.GetAll());
        }

        public void OnRecentClippingsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Clippings = new ObservableCollection<Clipping>(ClippingRepository.GetAll());

            NotifyOfPropertyChange(() => HasClippings);
        }
    }
}

namespace Omnipaste.ActivityList
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive.Linq;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.Activity;
    using Omnipaste.Helpers;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Properties;
    using Omnipaste.Services.Repositories;
    using OmniUI.List;

    public class ActivityListViewModel : ListViewModelBase<ActivityPresenter, IActivityViewModel>, IActivityListViewModel
    {
        #region Fields

        private readonly IClippingRepository _clippingRepository;

        private readonly IMessageRepository _messageRepository;

        private readonly ICallRepository _callRepository;

        private readonly IUpdateInfoRepository _updateInfoRepository;

        private readonly IActivityViewModelFactory _activityViewModelFactory;

        private ActivityTypeEnum _allowedActivityTypes;

        private string _filterText;

        private bool _showCalls;

        private bool _showClippings;

        private bool _showMessages;

        #endregion

        #region Constructors and Destructors

        public ActivityListViewModel(
            IClippingRepository clippingRepository,
            IMessageRepository messageRepository,
            ICallRepository callRepository,
            IUpdateInfoRepository updateInfoRepository,
            IActivityViewModelFactory activityViewModelFactory)
        {
            _clippingRepository = clippingRepository;
            _messageRepository = messageRepository;
            _callRepository = callRepository;
            _activityViewModelFactory = activityViewModelFactory;
            _updateInfoRepository = updateInfoRepository;
            _allowedActivityTypes = ActivityTypeEnum.All;
            
            FilteredItems.SortDescriptions.Add(new SortDescription("Time", ListSortDirection.Descending));
        }

        #endregion

        #region Public Properties

        public override string DisplayName
        {
            get
            {
                return Resources.Activity;
            }
        }

        public string FilterText
        {
            get
            {
                return _filterText;
            }
            set
            {
                if (value == _filterText)
                {
                    return;
                }
                _filterText = value;
                NotifyOfPropertyChange();
                UpdateFilter();
            }
        }

        public bool ShowCalls
        {
            get
            {
                return _showCalls;
            }
            set
            {
                if (value.Equals(_showCalls))
                {
                    return;
                }
                _showCalls = value;
                NotifyOfPropertyChange();
                UpdateFilter();
            }
        }

        public bool ShowClippings
        {
            get
            {
                return _showClippings;
            }
            set
            {
                if (value.Equals(_showClippings))
                {
                    return;
                }
                _showClippings = value;
                NotifyOfPropertyChange();
                UpdateFilter();
            }
        }

        public bool ShowMessages
        {
            get
            {
                return _showMessages;
            }
            set
            {
                if (value.Equals(_showMessages))
                {
                    return;
                }
                _showMessages = value;
                NotifyOfPropertyChange();
                UpdateFilter();
            }
        }

        #endregion

        #region Public Methods and Operators

        public void ShowVideoTutorial()
        {
            ExternalProcessHelper.ShowVideoTutorial();
        }

        #endregion

        #region Methods

        protected override IActivityViewModel CreateViewModel(ActivityPresenter model)
        {
            return _activityViewModelFactory.Create(model);
        }

        protected override bool CanShow(IActivityViewModel viewModel)
        {
            return MatchesActivityType(viewModel) && MatchesTextFilter(viewModel);
        }

        protected void UpdateFilter()
        {
            _allowedActivityTypes = ActivityTypeEnum.None;
            if (ShowClippings)
            {
                _allowedActivityTypes |= ActivityTypeEnum.Clipping;
            }

            if (ShowCalls)
            {
                _allowedActivityTypes |= ActivityTypeEnum.Call;
            }

            if (ShowMessages)
            {
                _allowedActivityTypes |= ActivityTypeEnum.Message;
            }

            if (_allowedActivityTypes == ActivityTypeEnum.None)
            {
                _allowedActivityTypes = ActivityTypeEnum.All;
            }

            RefreshItems();
        }

        protected override IObservable<IEnumerable<ActivityPresenter>> GetFetchItemsObservable()
        {
            return
                _clippingRepository.GetAll().Select(items => items.Select(item => new ActivityPresenter(item)))
                    .Merge(_messageRepository.GetAll().Select(items => items.Select(item => new ActivityPresenter(item))))
                    .Merge(_callRepository.GetAll().Select(items => items.Select(item => new ActivityPresenter(item))))
                    .Merge(_updateInfoRepository.GetAll().Select(items => items.Select(item => new ActivityPresenter(item))));
        }

        protected override IObservable<ActivityPresenter> GetItemAddedObservable()
        {
            return
                _clippingRepository.OperationObservable.Created().Select(o => new ActivityPresenter(o.Item))
                    .Merge(_messageRepository.OperationObservable.Created().Select(o => new ActivityPresenter(o.Item)))
                    .Merge(_callRepository.OperationObservable.Created().Select(o => new ActivityPresenter(o.Item)))
                    .Merge(_updateInfoRepository.OperationObservable.Created().Select(o => new ActivityPresenter(o.Item)));
        }

        protected override IObservable<ActivityPresenter> GetItemRemovedObservable()
        {
            return
                _clippingRepository.OperationObservable.Deleted().Select(o => GetActivity(ActivityTypeEnum.Clipping, o.Item.UniqueId))
                    .Merge(_messageRepository.OperationObservable.Deleted().Select(o => GetActivity(ActivityTypeEnum.Message, o.Item.UniqueId)))
                    .Merge(_callRepository.OperationObservable.Deleted().Select(o => GetActivity(ActivityTypeEnum.Call, o.Item.UniqueId)))
                    .Merge(_updateInfoRepository.OperationObservable.Deleted().Select(o => GetActivity(ActivityTypeEnum.Version, o.Item.UniqueId)));
        }

        private ActivityPresenter GetActivity(ActivityTypeEnum type, string id)
        {
            return
                Items.Select(vm => vm.Model)
                    .FirstOrDefault(activity => activity.Type == type && activity.SourceId == id);
        }

        private bool MatchesActivityType(IActivityViewModel viewModel)
        {
            return (viewModel != null) && _allowedActivityTypes.HasFlag(viewModel.Model.Type)
                   && (viewModel.Model.Type != ActivityTypeEnum.None) && (viewModel.Model.Type != ActivityTypeEnum.All);
        }

        private bool MatchesTextFilter(IActivityViewModel viewModel)
        {
            return string.IsNullOrWhiteSpace(FilterText)
                   || FilterText.Split(' ')
                          .Select(filterPart => filterPart.RemoveDiacritics())
                          .All(filterPart => viewModel.Model.ToString()
                                  .RemoveDiacritics()
                                  .IndexOf(filterPart, StringComparison.CurrentCultureIgnoreCase) >= 0);
        }

        #endregion
    }
}
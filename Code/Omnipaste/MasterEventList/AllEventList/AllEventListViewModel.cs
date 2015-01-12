namespace Omnipaste.MasterEventList.AllEventList
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using Ninject;
    using Omnipaste.DetailsViewModel;
    using Omnipaste.Event;
    using Omnipaste.MasterEventList.EventList;
    using Omnipaste.Services.Repositories;
    using OmniUI.Attributes;
    using OmniUI.List;

    [UseView(typeof(EventListView))]
    public class AllEventListViewModel : ListViewModelBase<IConversationItem, IEventViewModel>, IAllEventListViewModel
    {
        private readonly ICallRepository _callRepository;

        private readonly IMessageRepository _messageRepository;

        private readonly IKernel _kernel;
        
        public AllEventListViewModel(
            ICallRepository callRepository,
            IMessageRepository messageRepository,
            IKernel kernel)
        {
            _callRepository = callRepository;
            _messageRepository = messageRepository;
            _kernel = kernel;
        }

        protected override IEventViewModel CreateViewModel(IConversationItem model)
        {
            var eventViewModel = _kernel.Get<IEventViewModel>();
            eventViewModel.Model = model;

            return eventViewModel;
        }

        protected override IObservable<IEnumerable<IConversationItem>> GetFetchItemsObservable()
        {
            return _callRepository.GetAll()
                .Merge(_messageRepository.GetAll().Select(items => items.Cast<IConversationItem>()));
        }

        protected override IObservable<IConversationItem> GetItemAddedObservable()
        {
            return _callRepository.OperationObservable.Saved().Select(o => o.Item)
                .Merge(_messageRepository.OperationObservable.Saved().Select(o => o.Item).Cast<IConversationItem>());
        }

        protected override IObservable<IConversationItem> GetItemRemovedObservable()
        {
            return _callRepository.OperationObservable.Deleted().Select(o => o.Item)
                .Merge(_messageRepository.OperationObservable.Deleted().Select(o => o.Item).Cast<IConversationItem>());
        }
    }
}
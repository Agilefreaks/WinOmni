namespace Omnipaste.MasterEventList.IncomingSmsEventList
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using Ninject;
    using Omnipaste.Event;
    using Omnipaste.MasterEventList.EventList;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;
    using OmniUI.Attributes;
    using OmniUI.List;

    [UseView(typeof(EventListView))]
    public class IncomingSmsEventListViewModel : ListViewModelBase<Message, IEventViewModel>, IIncomingSmsEventListViewModel
    {
        private readonly IMessageRepository _messageRepository;

        private readonly IKernel _kernel;

        public IncomingSmsEventListViewModel(IMessageRepository messageRepository, IKernel kernel)
        {
            _messageRepository = messageRepository;
            _kernel = kernel;
        }

        protected override IObservable<IEnumerable<Message>> GetFetchItemsObservable()
        {
            return _messageRepository.GetAll();
        }

        protected override IObservable<Message> GetItemAddedObservable()
        {
            return _messageRepository.OperationObservable.Created().Select(o => o.Item);
        }

        protected override IObservable<Message> GetItemRemovedObservable()
        {
            return _messageRepository.OperationObservable.Deleted().Select(o => o.Item);
        }

        protected override IEventViewModel CreateViewModel(Message model)
        {
            var eventViewModel = _kernel.Get<IEventViewModel>();
            eventViewModel.Model = model;

            return eventViewModel;
        }
    }
}
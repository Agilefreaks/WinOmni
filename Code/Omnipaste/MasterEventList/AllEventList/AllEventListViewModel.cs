namespace Omnipaste.MasterEventList.AllEventList
{
    using System;
    using System.Reactive.Linq;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using Omnipaste.DetailsViewModel;
    using Omnipaste.Event;
    using Omnipaste.MasterEventList.EventList;
    using Omnipaste.Services.Repositories;
    using OmniUI.Attributes;
    using OmniUI.List;

    [UseView(typeof(EventListView))]
    public class AllEventListViewModel : ListViewModelBase<IConversationItem, IEventViewModel>, IAllEventListViewModel
    {
        private readonly IKernel _kernel;

        private readonly IDisposable _itemAddedSubscription;

        private readonly IDisposable _itemRemovedSubscription;

        public AllEventListViewModel(
            ICallRepository callRepository,
            IMessageRepository messageRepository,
            IKernel kernel)
        {
            _kernel = kernel;

            _itemAddedSubscription =
                GetItemAddedObservable(callRepository, messageRepository)
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Dispatcher)
                    .SubscribeAndHandleErrors(AddItem);
            _itemRemovedSubscription =
                GetItemRemovedObservable(callRepository, messageRepository)
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Dispatcher)
                    .SubscribeAndHandleErrors(RemoveItem);
        }

        public override void Dispose()
        {
            _itemAddedSubscription.Dispose();
            _itemRemovedSubscription.Dispose();
            base.Dispose();
        }

        protected override IEventViewModel CreateViewModel(IConversationItem model)
        {
            var eventViewModel = _kernel.Get<IEventViewModel>();
            eventViewModel.Model = model;

            return eventViewModel;
        }

        private IObservable<IConversationItem> GetItemAddedObservable(ICallRepository callRepository, IMessageRepository messageRepository)
        {
            return callRepository.OperationObservable.Saved()
                .Select(o => o.Item)
                .Merge(messageRepository.OperationObservable.Saved().Select(o => o.Item).Cast<IConversationItem>());
        }

        private IObservable<IConversationItem> GetItemRemovedObservable(ICallRepository callRepository, IMessageRepository messageRepository)
        {
            return callRepository.OperationObservable.Deleted()
                .Select(o => o.Item)
                .Merge(messageRepository.OperationObservable.Deleted().Select(o => o.Item).Cast<IConversationItem>());
        }
    }
}
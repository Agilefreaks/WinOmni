﻿namespace Omnipaste.MasterEventList.IncomingCallEventList
{
    using System;
    using System.Reactive.Linq;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using Omnipaste.Event;
    using Omnipaste.MasterEventList.EventList;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;
    using OmniUI.Attributes;
    using OmniUI.List;

    [UseView(typeof(EventListView))]
    public class IncomingCallEventListViewModel : ListViewModelBase<Call, IEventViewModel>, IIncomingCallEventListViewModel
    {
        private readonly IKernel _kernel;

        private readonly IDisposable _itemAddedSubscription;

        private readonly IDisposable _itemRemovedSubscription;

        public IncomingCallEventListViewModel(ICallRepository callRepository, IKernel kernel)
        {
            _kernel = kernel;

            _itemAddedSubscription =
                callRepository.OperationObservable.Saved()
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Dispatcher)
                    .SubscribeAndHandleErrors(o => AddItem(o.Item));
            _itemRemovedSubscription =
                callRepository.OperationObservable.Deleted()
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Dispatcher)
                    .SubscribeAndHandleErrors(o => RemoveItem(o.Item));
        }

        public override void Dispose()
        {
            _itemAddedSubscription.Dispose();
            _itemRemovedSubscription.Dispose();
            base.Dispose();
        }

        protected override IEventViewModel CreateViewModel(Call model)
        {
            var eventViewModel = _kernel.Get<IEventViewModel>();
            eventViewModel.Model = model;

            return eventViewModel;
        }
    }
}
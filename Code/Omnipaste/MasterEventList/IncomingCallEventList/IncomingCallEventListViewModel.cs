namespace Omnipaste.MasterEventList.IncomingCallEventList
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
    public class IncomingCallEventListViewModel : ListViewModelBase<Call, IEventViewModel>, IIncomingCallEventListViewModel
    {
        private readonly ICallRepository _callRepository;

        private readonly IKernel _kernel;

        public IncomingCallEventListViewModel(ICallRepository callRepository, IKernel kernel)
        {
            _callRepository = callRepository;
            _kernel = kernel;
        }

        protected override IObservable<IEnumerable<Call>> GetFetchItemsObservable()
        {
            return _callRepository.GetAll();
        }

        protected override IObservable<Call> GetItemAddedObservable()
        {
            return _callRepository.OperationObservable.Saved().Select(o => o.Item);
        }

        protected override IObservable<Call> GetItemRemovedObservable()
        {
            return _callRepository.OperationObservable.Deleted().Select(o => o.Item);
        }

        protected override IEventViewModel CreateViewModel(Call model)
        {
            var eventViewModel = _kernel.Get<IEventViewModel>();
            eventViewModel.Model = model;

            return eventViewModel;
        }
    }
}
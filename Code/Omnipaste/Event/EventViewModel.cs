namespace Omnipaste.Event
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Threading.Tasks;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using Ninject;
    using Omnipaste.DetailsViewModel;
    using Omnipaste.Dialog;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.MasterEventList.Calling;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;
    using OmniUI.Details;
    using PhoneCalls.Models;
    using PhoneCalls.Resources.v1;

    public class EventViewModel : DetailsViewModelBase<IConversationItem>, IEventViewModel
    {
        #region Constructors and Destructors

        public EventViewModel(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
        }

        #endregion

        #region Public Properties

        [Inject]
        public ICallRepository CallRepository { get; set; }

        [Inject]
        public ICallingViewModel CallingViewModel { get; set; }

        public string Content
        {
            get
            {
                return Model.Content;
            }
        }

        [Inject]
        public IDialogViewModel DialogViewModel { get; set; }

        public IEventAggregator EventAggregator { get; set; }

        [Inject]
        public IPhoneCalls PhoneCalls { get; set; }

        public DateTime Time
        {
            get
            {
                return Model.Time;
            }
        }

        public string Title
        {
            get
            {
                return string.IsNullOrWhiteSpace(Model.ContactInfo.Name)
                           ? Model.ContactInfo.Phone
                           : Model.ContactInfo.Name;
            }
        }

        public string Icon
        {
            get
            {
                return Model.GetType() == typeof(Call) ? "CallVisualBrush" : "EnvelopeVisualBrush";
            }
        }

        #endregion

        #region Public Methods and Operators

        public Task CallBack()
        {
            return
                PhoneCalls.Call(Model.ContactInfo.Phone)
                    .Select(SaveCallLocally)
                    .Switch()
                    .Do(_ => ShowCallingNotification())
                    .ToTask();
        }

        public void SendSms()
        {
            EventAggregator.PublishOnCurrentThread(new SendSmsMessage { Recipient = Model.ContactInfo.Phone });
        }

        #endregion

        #region Methods

        private IObservable<RepositoryOperation<Call>> SaveCallLocally(PhoneCall call)
        {
            return CallRepository.Save(new Call(call) { Source = SourceType.Local });
        }

        private void ShowCallingNotification()
        {
            DialogViewModel.ActivateItem(CallingViewModel);
        }

        #endregion
    }
}
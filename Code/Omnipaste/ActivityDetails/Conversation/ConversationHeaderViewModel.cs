namespace Omnipaste.ActivityDetails.Conversation
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using Ninject;
    using OmniApi.Resources.v1;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using Omnipaste.DetailsViewModel;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Services.Repositories;
    using OmniUI.Presenters;

    public class ConversationHeaderViewModel : ActivityDetailsHeaderViewModel, IConversationHeaderViewModel
    {
        #region Fields

        protected static TimeSpan DelayCallDuration = TimeSpan.FromSeconds(2);

        protected static TimeSpan CallingDuration = TimeSpan.FromSeconds(5);

        private IContactInfoPresenter _contactInfo;

        private ConversationHeaderStateEnum _state;

        private IDisposable _callSubscription;

        #endregion

        #region Public Properties

        public IContactInfoPresenter ContactInfo
        {
            get
            {
                return _contactInfo;
            }
            set
            {
                if (Equals(value, _contactInfo))
                {
                    return;
                }
                _contactInfo = value;
                NotifyOfPropertyChange();
            }
        }

        public TimeSpan ProgressDuration
        {
            get
            {
                return DelayCallDuration;
            }
        }

        public ConversationHeaderStateEnum State
        {
            get
            {
                return _state;
            }
            set
            {
                if (value == _state)
                {
                    return;
                }
                _state = value;
                NotifyOfPropertyChange();
            }
        }

        public override ActivityPresenter Model
        {
            get
            {
                return base.Model;
            }
            set
            {
                base.Model = value;
                ContactInfo = new ContactInfoPresenter(value.ExtraData.ContactInfo);
            }
        }

        [Inject]
        public IDevices Devices { get; set; }

        [Inject]
        public ICallRepository CallRepository { get; set; }

        [Inject]
        public IMessageRepository MessageRepository { get; set; }

        #endregion

        public void Call()
        {
            DisposeCallSubscription();
            _callSubscription = Observable.Interval(DelayCallDuration, SchedulerProvider.Default)
                .Take(1, SchedulerProvider.Default)
                .Do(_ => { State = ConversationHeaderStateEnum.Calling; })
                .Select(_ => Devices.Call(Model.ExtraData.ContactInfo.Phone as string))
                .Switch()
                .Delay(CallingDuration, SchedulerProvider.Default)
                .Do(_ =>  { State = ConversationHeaderStateEnum.Normal; })
                .SubscribeAndHandleErrors();

            State = ConversationHeaderStateEnum.InitiatingCall;
        }

        public void CancelCall()
        {
            DisposeCallSubscription();

            State = ConversationHeaderStateEnum.Normal;
        }

        public void Delete()
        {
            Model.IsDeleted = true;
            UpdateConversationItems(CallRepository, item => item.IsDeleted = true);
            UpdateConversationItems(MessageRepository, item => item.IsDeleted = true);
            State = ConversationHeaderStateEnum.Deleted;
        }

        public void UndoDelete()
        {
            Model.IsDeleted = false;
            UpdateConversationItems(CallRepository, item => item.IsDeleted = false);
            UpdateConversationItems(MessageRepository, item => item.IsDeleted = false);
            State = ConversationHeaderStateEnum.Normal;
        }

        protected override void OnActivate()
        {
            State = ConversationHeaderStateEnum.Normal;
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            DeleteConversationItems(CallRepository);
            DeleteConversationItems(MessageRepository);
            base.OnDeactivate(close);
        }

        private void UpdateConversationItems<T>(IRepository<T> repository, Action<T> update) where T : BaseModel, IConversationItem
        {
            repository.GetByContact(ContactInfo.ContactInfo)
                .SubscribeAndHandleErrors(
                    items => items.ToList().ForEach(
                        i =>
                            {
                                update(i);
                                repository.Save(i).RunToCompletion();
                            }));
        }

        private void DeleteConversationItems<T>(IRepository<T> repository) where T : BaseModel, IConversationItem
        {
            repository.GetByContact(ContactInfo.ContactInfo)
                .SubscribeAndHandleErrors(
                    items =>
                    items.Where(c => c.IsDeleted).ToList().ForEach(c => repository.Delete(c.UniqueId).RunToCompletion()));
        }

        private void DisposeCallSubscription()
        {
            if (_callSubscription == null)
            {
                return;
            }

            _callSubscription.Dispose();
            _callSubscription = null;
        }
    }
}
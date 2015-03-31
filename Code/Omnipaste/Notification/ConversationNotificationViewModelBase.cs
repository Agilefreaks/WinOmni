namespace Omnipaste.Notification
{
    using System.Reactive.Linq;
    using System.Reactive.Threading.Tasks;
    using System.Threading.Tasks;
    using OmniCommon.Helpers;
    using Omnipaste.Framework.Commands;
    using Omnipaste.Models;
    using OmniUI.Services;

    public abstract class ConversationNotificationViewModelBase : ResourceBasedNotificationViewModel<IConversationModel>,
                                                                  IConversationNotificationViewModel
    {
        #region Fields

        private readonly ICommandService _commandService;

        private bool _canReplyWithSms = true;

        #endregion

        #region Constructors and Destructors

        protected ConversationNotificationViewModelBase(ICommandService commandService)
        {
            _commandService = commandService;
        }

        #endregion

        #region Public Properties

        public bool CanReplyWithSMS
        {
            get
            {
                return _canReplyWithSms;
            }
            set
            {
                if (value.Equals(_canReplyWithSms))
                {
                    return;
                }
                _canReplyWithSms = value;
                NotifyOfPropertyChange();
            }
        }

        public override object Identifier
        {
            get
            {
                return Resource.UniqueId;
            }
        }

        public override string Line1
        {
            get
            {
                return Resource.ContactModel.Identifier;
            }
        }

        public override string Line2
        {
            get
            {
                return Resource.Content;
            }
        }

        #endregion

        #region Public Methods and Operators

        public async Task ReplyWithSMS()
        {
            CanReplyWithSMS = false;

            var command = new ComposeSMSCommand(Resource.ContactModel);
            await _commandService.Execute(command).SubscribeOn(SchedulerProvider.Dispatcher).ToTask();

            Dismiss();
        }

        #endregion
    }
}
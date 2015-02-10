﻿namespace Omnipaste.Notification.IncomingSmsNotification
{
    using Omnipaste.Properties;
    using OmniUI.Services;

    public class IncomingSmsNotificationViewModel : ConversationNotificationViewModelBase, IIncomingSmsNotificationViewModel
    {
        #region Constructors and Destructors

        public IncomingSmsNotificationViewModel(ICommandService commandService)
            : base(commandService)
        {
        }

        #endregion

        #region Public Properties

        public override string Title
        {
            get
            {
                return Resources.IncommingSmsNotificationTitle;
            }
        }

        public override NotificationTypeEnum Type
        {
            get
            {
                return NotificationTypeEnum.IncomingSMS;
            }
        }

        #endregion
    }
}
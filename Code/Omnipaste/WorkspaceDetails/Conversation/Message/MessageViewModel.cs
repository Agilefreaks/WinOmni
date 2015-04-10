﻿namespace Omnipaste.WorkspaceDetails.Conversation.Message
{
    using System;
    using OmniCommon.Interfaces;
    using Omnipaste.Framework.Models;
    using Omnipaste.Services;

    public class MessageViewModel : ConversationItemViewModel<SmsMessageModel>, IMessageViewModel
    {
        #region Constructors and Destructors

        public MessageViewModel(IUiRefreshService uiRefreshService, IConfigurationService configurationService)
            : base(uiRefreshService, configurationService)
        {
        }

        #endregion

        public DateTime Time
        {
            get
            {
                return Model.Time;
            }
        }
    }
}
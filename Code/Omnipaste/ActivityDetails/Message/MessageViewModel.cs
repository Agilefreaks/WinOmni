﻿namespace Omnipaste.ActivityDetails.Message
{
    using Omnipaste.DetailsViewModel;
    using Omnipaste.Models;
    using Omnipaste.Services;

    public class MessageViewModel : DetailsViewModelWithContact<Message>, IMessageViewModel
    {
        #region Constructors and Destructors

        public MessageViewModel(IUiRefreshService uiRefreshService)
            : base(uiRefreshService)
        {
        }

        #endregion
    }
}
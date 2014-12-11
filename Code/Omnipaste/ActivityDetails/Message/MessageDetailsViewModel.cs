﻿namespace Omnipaste.ActivityDetails.Message
{
    using OmniUI.Attributes;

    [UseView("Omnipaste.ActivityDetails.ActivityDetailsView", IsFullyQualifiedName = true)]
    public class MessageDetailsViewModel : ActivityDetailsViewModel, IMessageDetailsViewModel
    {
        #region Constructors and Destructors

        public MessageDetailsViewModel(
            IMessageDetailsHeaderViewModel headerViewModel,
            IActivityDetailsContentViewModel contentViewModel)
            : base(headerViewModel, contentViewModel)
        {
        }

        #endregion
    }
}
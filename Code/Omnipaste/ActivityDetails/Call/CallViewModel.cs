﻿namespace Omnipaste.ActivityDetails.Call
{
    using Omnipaste.DetailsViewModel;
    using Omnipaste.Models;
    using Omnipaste.Services;

    public class CallViewModel : DetailsViewModelWithContact<Call>, ICallViewModel
    {
        #region Constructors and Destructors

        public CallViewModel(IUiRefreshService uiRefreshService)
            : base(uiRefreshService)
        {
        }

        #endregion
    }
}
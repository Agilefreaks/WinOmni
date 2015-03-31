namespace Omnipaste.Activity
{
    using System;
    using Omnipaste.DetailsViewModel;
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using Omnipaste.Presenters;

    public interface IActivityViewModel : IDetailsViewModelWithAutoRefresh<ActivityPresenter>
    {
        ActivityTypeEnum ActivityType { get; }

        ActivityContentInfo ContentInfo { get; set; }

        DateTime Time { get; }
    }
}
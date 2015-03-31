namespace Omnipaste.Activity
{
    using System;
    using Omnipaste.DetailsViewModel;
    using Omnipaste.Models;

    public interface IActivityViewModel : IDetailsViewModelWithAutoRefresh<ActivityModel>
    {
        ActivityTypeEnum ActivityType { get; }

        ActivityContentInfo ContentInfo { get; set; }

        DateTime Time { get; }
    }
}
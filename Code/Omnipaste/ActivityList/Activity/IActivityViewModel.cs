namespace Omnipaste.ActivityList.Activity
{
    using System;
    using Omnipaste.Framework.DetailsViewModel;
    using Omnipaste.Framework.Models;

    public interface IActivityViewModel : IDetailsViewModelWithAutoRefresh<ActivityModel>
    {
        ActivityTypeEnum ActivityType { get; }

        ActivityContentInfo ContentInfo { get; set; }

        DateTime Time { get; }
    }
}
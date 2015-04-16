namespace Omnipaste.Activities.ActivityList.Activity
{
    using System;
    using Omnipaste.Framework.Models;
    using OmniUI.Details;

    public interface IActivityViewModel : IDetailsViewModelWithAutoRefresh<ActivityModel>
    {
        ActivityTypeEnum ActivityType { get; }

        ActivityContentInfo ContentInfo { get; set; }

        DateTime Time { get; }
    }
}
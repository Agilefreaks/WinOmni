namespace Omnipaste.Activity
{
    using System;
    using Omnipaste.DetailsViewModel;
    using Omnipaste.Presenters;

    public interface IActivityViewModel : IDetailsViewModelWithAutoRefresh<ActivityPresenter>
    {
        ContentTypeEnum ContentType { get; set; }

        ActivityContentInfo ContentInfo { get; set; }

        DateTime Time { get; }
    }
}
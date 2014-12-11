namespace Omnipaste.Activity
{
    using Omnipaste.Activity.Presenters;

    public interface IContactRelatedActivityViewModel : IActivityViewModel
    {
        IContactInfoPresenter ContactInfo { get; set; }
    }
}
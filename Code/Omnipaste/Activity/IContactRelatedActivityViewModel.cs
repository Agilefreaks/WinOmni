namespace Omnipaste.Activity
{
    using Omnipaste.Presenters;

    public interface IContactRelatedActivityViewModel : IActivityViewModel
    {
        IContactInfoPresenter ContactInfo { get; set; }
    }
}
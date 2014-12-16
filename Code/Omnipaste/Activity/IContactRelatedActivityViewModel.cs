namespace Omnipaste.Activity
{
    using OmniUI.Presenters;

    public interface IContactRelatedActivityViewModel : IActivityViewModel
    {
        IContactInfoPresenter ContactInfo { get; set; }
    }
}
namespace Omnipaste.ActivityDetails
{
    using Omnipaste.Presenters;
    using OmniUI.Details;

    public interface IActivityDetailsViewModel : IDetailsViewModelWithHeader<IActivityDetailsHeaderViewModel, IActivityDetailsContentViewModel>
    {
        ActivityPresenter Model { get; set; }
    }
}
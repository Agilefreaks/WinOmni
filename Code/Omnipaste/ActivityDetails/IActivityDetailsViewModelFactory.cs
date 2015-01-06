namespace Omnipaste.ActivityDetails
{
    using Omnipaste.Presenters;

    public interface IActivityDetailsViewModelFactory
    {
        IActivityDetailsViewModel Create(ActivityPresenter activity);
    }
}
namespace Omnipaste.Activity
{
    using Omnipaste.Presenters;

    public interface IActivityViewModelFactory
    {
        IActivityViewModel Create(ActivityPresenter activity);
    }
}
namespace Omnipaste.Activity
{
    public interface IActivityViewModelFactory
    {
        IActivityViewModel Create(Models.Activity activity);
    }
}
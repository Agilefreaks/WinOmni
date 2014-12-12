namespace Omnipaste.Activity
{
    public interface IActivityViewModelFactory
    {
        IActivityViewModel Create(Omnipaste.Models.Activity activity);
    }
}
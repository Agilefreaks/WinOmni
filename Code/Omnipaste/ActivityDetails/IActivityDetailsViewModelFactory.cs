namespace Omnipaste.ActivityDetails
{
    using Omnipaste.Models;

    public interface IActivityDetailsViewModelFactory
    {
        IActivityDetailsViewModel Create(Activity activity);
    }
}
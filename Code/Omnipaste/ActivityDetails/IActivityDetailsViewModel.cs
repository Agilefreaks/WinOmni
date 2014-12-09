namespace Omnipaste.ActivityDetails
{
    using Caliburn.Micro;

    public interface IActivityDetailsViewModel : IScreen
    {
        IActivityDetailsContentViewModel ContentViewModel { get; }

        IActivityDetailsHeaderViewModel HeaderViewModel { get; }
    }
}
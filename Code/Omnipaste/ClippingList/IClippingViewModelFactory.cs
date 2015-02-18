namespace Omnipaste.ClippingList
{
    using Omnipaste.Presenters;

    public interface IClippingViewModelFactory
    {
        IClippingViewModel Create(ClippingPresenter contactInfoPresenter);
    }
}
namespace Omnipaste.ClippingList
{
    using Omnipaste.Models;

    public interface IClippingViewModelFactory
    {
        IClippingViewModel Create(ClippingModel contactInfoModel);
    }
}
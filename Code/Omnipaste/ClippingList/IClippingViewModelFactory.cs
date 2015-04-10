namespace Omnipaste.ClippingList
{
    using Omnipaste.ClippingList.Clipping;
    using Omnipaste.Models;

    public interface IClippingViewModelFactory
    {
        IClippingViewModel Create(ClippingModel clippingModel);
    }
}
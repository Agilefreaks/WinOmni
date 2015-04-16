namespace Omnipaste.Clippings.ClippingList
{
    using Omnipaste.Clippings.ClippingList.Clipping;
    using Omnipaste.Framework.Models;

    public interface IClippingViewModelFactory
    {
        IClippingViewModel Create(ClippingModel clippingModel);
    }
}
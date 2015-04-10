namespace Omnipaste.ClippingList
{
    using Omnipaste.ClippingList.Clipping;
    using Omnipaste.Framework.Models;

    public interface IClippingViewModelFactory
    {
        IClippingViewModel Create(ClippingModel clippingModel);
    }
}
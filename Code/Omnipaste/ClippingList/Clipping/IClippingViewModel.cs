namespace Omnipaste.ClippingList.Clipping
{
    using System;
    using Omnipaste.Models;
    using OmniUI.Details;

    public interface IClippingViewModel : IDetailsViewModel<ClippingModel>
    {
        DateTime Time { get; }
    }
}
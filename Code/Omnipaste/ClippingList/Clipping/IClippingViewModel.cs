namespace Omnipaste.ClippingList.Clipping
{
    using System;
    using Omnipaste.Framework.Models;
    using OmniUI.Details;

    public interface IClippingViewModel : IDetailsViewModel<ClippingModel>
    {
        DateTime Time { get; }
    }
}
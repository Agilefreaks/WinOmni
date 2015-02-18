namespace Omnipaste.ClippingList
{
    using System;
    using Omnipaste.Presenters;
    using OmniUI.Details;

    public interface IClippingViewModel : IDetailsViewModel<ClippingPresenter>
    {
        DateTime Time { get; }
    }
}
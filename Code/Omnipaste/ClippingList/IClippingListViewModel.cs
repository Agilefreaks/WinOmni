namespace Omnipaste.ClippingList
{
    using System;
    using Caliburn.Micro;
    using Clipboard.Models;

    public interface IClippingListViewModel : IScreen, IObserver<Clipping>
    {
    }
}
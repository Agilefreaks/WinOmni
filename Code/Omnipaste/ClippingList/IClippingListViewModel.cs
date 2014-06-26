namespace Omnipaste.ClippingList
{
    using System;
    using Caliburn.Micro;
    using Clipboard.Models;
    using Omnipaste.Clipping;

    public interface IClippingListViewModel
    {
        IObservableCollection<IClippingViewModel> Clippings { get; set; }

        IObservable<Clipping> ClippingsObservable { get; set; }
    }
}
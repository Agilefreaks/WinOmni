namespace Omnipaste.ClippingList
{
    using System;
    using Caliburn.Micro;
    using Clipboard.Handlers;
    using Clipboard.Models;

    public class ClippingListViewModel : Screen, IClippingListViewModel
    {
        public IClipboardHandler ClipboardHandler { get; set; }

        public ILocalClipboardHandler LocalClipboardHandler { get; set; }

        public IOmniClipboardHandler OmniClipboardHandler { get; set; }

        public IObservableCollection<Clipping> Clippings { get; set; } 

        public ClippingListViewModel(IClipboardHandler clipboardHandler)
        {
            Clippings = new BindableCollection<Clipping>();

            ClipboardHandler = clipboardHandler;
            ClipboardHandler.Subscribe(this);
        }

        public void OnNext(Clipping value)
        {
            Clippings.Add(value);
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }
    }
}
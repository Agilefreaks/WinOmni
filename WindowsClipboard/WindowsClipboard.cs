using System;
using System.Reactive.Linq;
using WindowsClipboard.Interfaces;
using Caliburn.Micro;
using Ninject;
using OmniCommon.Models;
using OmniCommon.Services;

namespace WindowsClipboard
{
    public class WindowsClipboard : IWindowsClipboard
    {
        public IObservable<ClipboardData> Clippings { get; set; }

        [Inject]
        public IWindowsClipboardWrapper WindowsClipboardWrapper { get; set; }

        [Inject]
        public IEventAggregator EventAggregator { get; set; }

        public void Start()
        {
            EventAggregator.Subscribe(this) ;
            WindowsClipboardWrapper.StartWatchingClipboard();
            
            Clippings =
                Observable.FromEventPattern<ClipboardEventArgs>(
                    h => WindowsClipboardWrapper.DataReceived += h,
                    h => WindowsClipboardWrapper.DataReceived -= h)
                    .Select(i => new ClipboardData(i.Sender, i.EventArgs.Data));
        }

        public void Stop()
        {
            WindowsClipboardWrapper.StopWatchingClipboard();
            EventAggregator.Unsubscribe(this);
        }

        public void Handle(Clipping clipping)
        {
            PutData(clipping.Content);
        }

        private void PutData(string data)
        {
            WindowsClipboardWrapper.SetData(data);
        }
    }
}
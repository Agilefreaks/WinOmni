using System;

namespace ClipboardWatcher.Core
{
    public class ClipboardEventArgs : EventArgs
    {
        public string Data { get; set; }
    }
}
﻿using System;

namespace WindowsClipboard.Interfaces
{
    public interface IWindowsClipboardWrapper
    {
        event EventHandler<ClipboardEventArgs> DataReceived;

        void SetData(string data);

        void StartWatchingClipboard();

        void StopWatchingClipboard();
    }
}
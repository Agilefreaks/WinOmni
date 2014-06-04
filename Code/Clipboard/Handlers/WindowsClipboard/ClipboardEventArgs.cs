namespace Clipboard.Handlers.WindowsClipboard
{
    using System;

    public class ClipboardEventArgs : EventArgs
    {
        public string Data { get; set; }

        public ClipboardEventArgs()
        {
        }

        public static bool Equals(ClipboardEventArgs firstEventArgs, ClipboardEventArgs secondEventArgs)
        {
            // ReSharper disable PossibleUnintendedReferenceComparison       
            return (firstEventArgs == secondEventArgs) ||
                   (firstEventArgs != null && secondEventArgs != null && firstEventArgs.Equals(secondEventArgs));
            // ReSharper restore PossibleUnintendedReferenceComparison
        }

        public ClipboardEventArgs(string data)
        {
            Data = data;
        }

        public override int GetHashCode()
        {
            return Data != null ? Data.GetHashCode() : 0;
        }

        public override bool Equals(object obj)
        {
            var eventArgs = obj as ClipboardEventArgs;

            return eventArgs != null && eventArgs.Data.Equals(Data);
        }
    }
}
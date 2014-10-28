namespace OmniCommon.Interfaces
{
    using System;

    public interface IWindowHandleProvider : IObservable<IntPtr>
    {
        void SetHandle(IntPtr windowHandle);
    }
}
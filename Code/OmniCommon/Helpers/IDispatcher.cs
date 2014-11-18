namespace OmniCommon.Helpers
{
    using System;

    public interface IDispatcher
    {
        void Dispatch(Delegate method, params object[] arguments);
    }
}
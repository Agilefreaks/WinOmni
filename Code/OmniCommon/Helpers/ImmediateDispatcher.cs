namespace OmniCommon.Helpers
{
    using System;

    public class ImmediateDispatcher : IDispatcher
    {
        public void Dispatch(Delegate method, params object[] arguments)
        {
            method.DynamicInvoke(arguments);
        }
    }
}
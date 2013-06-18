namespace Omnipaste
{
    using Caliburn.Micro;
    using OmniCommon.EventAggregatorMessages;
    using WindowsClipboard.Interfaces;

    public interface IMainForm : IDelegateClipboardMessageHandling, IHandle<OmniServiceStatusChanged>
    {
    }
}
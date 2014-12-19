namespace OmniHolidays.MessagesWorkspace.MessageDetails
{
    using System;
    using Caliburn.Micro;

    public interface IMessageStepViewModel : IScreen
    {
        event EventHandler<EventArgs> OnNext;

        event EventHandler<EventArgs> OnPrevious;

        MessageContext MessageContext { get; set; }
    }
}
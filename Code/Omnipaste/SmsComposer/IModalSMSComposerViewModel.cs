namespace Omnipaste.SMSComposer
{
    using Caliburn.Micro;
    using Omnipaste.EventAggregatorMessages;

    public interface IModalSMSComposerViewModel : ISMSComposerViewModel, IHandle<SendSmsMessage>
    {
        SMSComposerStatusEnum State { get; set; }
    }
}
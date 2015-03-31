namespace OmniDebug.Services
{
    using SMS.Dto;
    using SMS.Resources.v1;

    public interface ISmsMessagesWrapper : ISMSMessages
    {
        void MockGet(string id, SmsMessageDto phoneCall);
    }
}
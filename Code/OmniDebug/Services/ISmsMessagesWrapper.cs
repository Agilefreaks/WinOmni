namespace OmniDebug.Services
{
    using SMS.Models;
    using SMS.Resources.v1;

    public interface ISmsMessagesWrapper : ISMSMessages
    {
        void MockGet(string id, SmsMessageDto phoneCall);
    }
}
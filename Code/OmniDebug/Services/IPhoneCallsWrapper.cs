namespace OmniDebug.Services
{
    using PhoneCalls.Models;
    using PhoneCalls.Resources.v1;

    public interface IPhoneCallsWrapper : IPhoneCalls
    {
        void MockGet(string id, PhoneCall phoneCall);
    }
}
namespace OmniDebug.Services
{
    using PhoneCalls.Dto;
    using PhoneCalls.Resources.v1;

    public interface IPhoneCallsWrapper : IPhoneCalls
    {
        void MockGet(string id, PhoneCallDto phoneCallDto);
    }
}
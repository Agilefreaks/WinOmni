namespace Omniclipboard.Services
{
    public class MockActivationDataProvider : IActivationDataProvider
    {
        public ActivationData GetActivationData()
        {
            return new ActivationData { Channel = "test@email.com" };
        }
    }
}
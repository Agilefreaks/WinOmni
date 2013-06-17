namespace Omnipaste.Services
{
    public class MockActivationDataProvider : IActivationDataProvider
    {
        public ActivationData GetActivationData(string token)
        {
            return new ActivationData { Email = "test@email.com" };
        }
    }
}
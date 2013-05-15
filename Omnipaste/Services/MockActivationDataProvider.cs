using OmniCommon.Interfaces;
using OmniCommon.Services;

namespace Omnipaste.Services
{
    public class MockActivationDataProvider : IActivationDataProvider
    {
        public ActivationData GetActivationData()
        {
            return new ActivationData { Email = "test@email.com" };
        }
    }
}
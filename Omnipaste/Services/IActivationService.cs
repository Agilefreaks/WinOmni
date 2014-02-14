using System.Threading.Tasks;

namespace Omnipaste.Services
{
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    public interface IActivationService
    {
        IActivationStep CurrentStep { get; }

        Task Run();
    }
}
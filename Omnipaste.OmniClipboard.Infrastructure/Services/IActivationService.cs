namespace Omnipaste.OmniClipboard.Infrastructure.Services
{
    using Omnipaste.OmniClipboard.Infrastructure.Services.ActivationServiceData.ActivationServiceSteps;

    public interface IActivationService
    {
        IActivationStep CurrentStep { get; }

        void Run();

        void MoveToNextStep();

        IActivationStep GetNextStep();
    }
}
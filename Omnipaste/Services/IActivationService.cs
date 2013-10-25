namespace Omnipaste.Services
{
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    public interface IActivationService
    {
        IActivationStep CurrentStep { get; }

        void Run();

        void MoveToNextStep();

        IActivationStep GetNextStep();
    }
}
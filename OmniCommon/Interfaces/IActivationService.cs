namespace OmniCommon.Interfaces
{
    using OmniCommon.Services.ActivationServiceData;
    using OmniCommon.Services.ActivationServiceData.ActivationServiceSteps;

    public interface IActivationService
    {
        IActivationStep CurrentStep { get; }

        IStepFactory StepFactory { get; set; }

        void Run();

        void MoveToNextStep();

        IActivationStep GetNextStep();
    }
}
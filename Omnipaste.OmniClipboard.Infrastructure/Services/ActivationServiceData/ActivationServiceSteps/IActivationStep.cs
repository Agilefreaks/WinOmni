namespace Omnipaste.OmniClipboard.Infrastructure.Services.ActivationServiceData.ActivationServiceSteps
{
    using Omnipaste.OmniClipboard.Infrastructure.Services.ActivationServiceData;

    public interface IActivationStep
    {
        DependencyParameter Parameter { get; set; }

        IExecuteResult Execute();

        object GetId();
    }
}
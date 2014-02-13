using System.Threading.Tasks;

namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    public interface IActivationStep
    {
        DependencyParameter Parameter { get; set; }

        IExecuteResult Execute();

        object GetId();
    }
}
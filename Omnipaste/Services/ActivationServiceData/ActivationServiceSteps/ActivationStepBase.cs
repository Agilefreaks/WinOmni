using System.Threading.Tasks;

namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    public abstract class ActivationStepBase : IActivationStep
    {
        public virtual DependencyParameter Parameter { get; set; }

        public abstract IExecuteResult Execute();

        public virtual Task<IExecuteResult> ExecuteAsync()
        {
            return Task.Factory.StartNew(() => Execute());
        }

        public virtual object GetId()
        {
            return this.GetType();
        }
    }
}
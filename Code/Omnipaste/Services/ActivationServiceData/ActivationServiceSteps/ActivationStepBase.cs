namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;

    public abstract class ActivationStepBase : IActivationStep
    {
        #region Public Properties

        public virtual DependencyParameter Parameter { get; set; }

        #endregion

        #region Public Methods and Operators

        public virtual object GetId()
        {
            return GetType();
        }

        public abstract IObservable<IExecuteResult> Execute();

        #endregion
    }
}
namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;

    public abstract class ActivationStepBase : IActivationStep
    {
        #region Public Properties

        public virtual DependencyParameter Parameter { get; set; }

        #endregion

        #region Public Methods and Operators

        public IObservable<IExecuteResult> Execute()
        {
            return InternalExecute();
        }

        public virtual object GetId()
        {
            return GetType();
        }

        #endregion

        #region Methods

        protected abstract IObservable<IExecuteResult> InternalExecute();

        #endregion
    }
}
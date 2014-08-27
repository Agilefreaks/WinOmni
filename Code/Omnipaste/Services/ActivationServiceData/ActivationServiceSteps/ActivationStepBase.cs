namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive.Linq;

    public abstract class ActivationStepBase : IActivationStep
    {
        #region Public Properties

        public virtual DependencyParameter Parameter { get; set; }

        #endregion

        #region Public Methods and Operators

        public IObservable<IExecuteResult> Execute()
        {
            return InternalExecute().Catch<IExecuteResult, Exception>(GetFailObserver);
        }

        public IObservable<IExecuteResult> GetFailObserver(Exception exception)
        {
            return new[] { new ExecuteResult(SimpleStepStateEnum.Failed, exception) }.ToObservable();
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
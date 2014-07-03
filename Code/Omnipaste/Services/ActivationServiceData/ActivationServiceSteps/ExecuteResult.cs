namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    public class ExecuteResult : IExecuteResult
    {
        #region Constructors and Destructors

        public ExecuteResult()
        {
        }

        public ExecuteResult(SimpleStepStateEnum state)
        {
            State = state;
        }

        public ExecuteResult(SimpleStepStateEnum state, object data) : this(state)
        {
            Data = data;
        }

        #endregion

        #region Public Properties

        public object Data { get; set; }

        public SimpleStepStateEnum State { get; set; }

        #endregion
    }
}
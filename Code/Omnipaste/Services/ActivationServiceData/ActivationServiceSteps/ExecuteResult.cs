namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    public class ExecuteResult : IExecuteResult
    {
        #region Constructors and Destructors

        public ExecuteResult()
        {
        }

        public ExecuteResult(object state)
        {
            State = state;
        }

        public ExecuteResult(object state, object data)
            : this(state)
        {
            Data = data;
        }

        #endregion

        #region Public Properties

        public object Data { get; set; }

        public object State { get; set; }

        #endregion
    }
}
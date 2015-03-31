namespace Omnipaste.Framework.Services.ActivationServiceData.Transitions
{
    public class TransitionId
    {
        #region Constructors and Destructors

        public TransitionId(object stateId, object state)
        {
            StateId = stateId;
            State = state;
        }

        #endregion

        #region Public Properties

        public object State { get; private set; }

        public object StateId { get; private set; }

        #endregion

        #region Public Methods and Operators

        public override bool Equals(object obj)
        {
            return Equals(obj as TransitionId);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((StateId != null ? StateId.GetHashCode() : 0) * 397) ^ (State != null ? State.GetHashCode() : 0);
            }
        }

        #endregion

        #region Methods

        protected bool Equals(TransitionId other)
        {
            return other != null && Equals(StateId, other.StateId) && Equals(State, other.State);
        }

        #endregion
    }
}
namespace OmniCommon.Services.TransitionId
{
    public class TransitionId
    {
        public object StateId { get; private set; }

        public object State { get; private set; }

        public TransitionId(object stateId, object state)
        {
            StateId = stateId;
            State = state;
        }

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

        protected bool Equals(TransitionId other)
        {
            return other != null && StateId == other.StateId && Equals(State, other.State);
        }
    }
}
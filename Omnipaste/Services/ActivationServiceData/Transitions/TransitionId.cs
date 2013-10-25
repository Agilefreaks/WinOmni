namespace Omnipaste.Services.ActivationServiceData.Transitions
{
    public class TransitionId
    {
        public object StateId { get; private set; }

        public object State { get; private set; }

        public TransitionId(object stateId, object state)
        {
            this.StateId = stateId;
            this.State = state;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as TransitionId);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.StateId != null ? this.StateId.GetHashCode() : 0) * 397) ^ (this.State != null ? this.State.GetHashCode() : 0);
            }
        }

        protected bool Equals(TransitionId other)
        {
            return other != null && Equals(this.StateId, other.StateId) && Equals(this.State, other.State);
        }
    }
}
namespace Omnipaste.Services.ActivationServiceData.Transitions
{
    public class GenericTransitionId<T> : TransitionId
    {
        private GenericTransitionId(object state)
            : base(typeof(T), state)
        {
        }

        public static GenericTransitionId<T> Create<TStateType>(TStateType state)
        {
            return new GenericTransitionId<T>(state);
        }
    }
}
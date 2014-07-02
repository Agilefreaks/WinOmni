namespace Omnipaste.Services.ActivationServiceData.Transitions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    public class TransitionCollection
    {
        #region Fields

        private readonly ImmutableDictionary<TransitionId, Type> _transitionDictionary;

        #endregion

        #region Constructors and Destructors

        private TransitionCollection(Dictionary<TransitionId, Type> transitions)
        {
            _transitionDictionary = transitions.ToImmutableDictionary();
        }

        #endregion

        #region Public Methods and Operators

        public static TransitionCollectionBuilder Builder()
        {
            return new TransitionCollectionBuilder();
        }

        public Type GetTargetTypeForTransition(TransitionId transitionId)
        {
            return _transitionDictionary.ContainsKey(transitionId) ? _transitionDictionary[transitionId] : null;
        }

        public Type GetTargetTypeForTransition<TSource>(SimpleStepStateEnum state)
        {
            var transitionId = GenericTransitionId<TSource>.Create(state);
            return GetTargetTypeForTransition(transitionId);
        }

        #endregion

        public class TransitionCollectionBuilder
        {
            #region Fields

            private readonly Dictionary<TransitionId, Type> _transitionDictionary;

            #endregion

            #region Constructors and Destructors

            public TransitionCollectionBuilder()
            {
                _transitionDictionary = new Dictionary<TransitionId, Type>();
            }

            #endregion

            #region Public Methods and Operators

            public TransitionCollection Build()
            {
                return new TransitionCollection(_transitionDictionary);
            }

            public TransitionCollectionBuilder RegisterTransition<TSource, TSuccess>()
            {
                _transitionDictionary.Add(GenericTransitionId<TSource>.Create(SimpleStepStateEnum.Successful), typeof(TSuccess));
                return this;
            }

            public TransitionCollectionBuilder RegisterTransition<TSource, TSuccess, TFailure>()
            {
                RegisterTransition<TSource, TSuccess>();
                _transitionDictionary.Add(GenericTransitionId<TSource>.Create(SimpleStepStateEnum.Failed), typeof(TFailure));
                return this;
            }

            #endregion
        }
    }
}
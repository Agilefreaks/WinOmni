namespace OmniCommon.Services.ActivationServiceData.Transitions
{
    using System;
    using System.Collections.Generic;

    public class TransitionCollection
    {
        private readonly Dictionary<TransitionId, Type> _transitionDictionary;

        public TransitionCollection()
        {
            _transitionDictionary = new Dictionary<TransitionId, Type>();
        }

        public void RegisterTransition(TransitionId transitionId, Type targetStateType)
        {
            _transitionDictionary.Add(transitionId, targetStateType);
        }

        public Type GetTargetTypeForTransition(TransitionId transitionId)
        {
            return _transitionDictionary.ContainsKey(transitionId) ? _transitionDictionary[transitionId] : null;
        }
    }
}
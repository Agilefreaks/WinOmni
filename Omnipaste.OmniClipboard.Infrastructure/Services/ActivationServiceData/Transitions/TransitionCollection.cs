namespace Omnipaste.OmniClipboard.Infrastructure.Services.ActivationServiceData.Transitions
{
    using System;
    using System.Collections.Generic;

    public class TransitionCollection
    {
        private readonly Dictionary<TransitionId, Type> _transitionDictionary;

        public TransitionCollection()
        {
            this._transitionDictionary = new Dictionary<TransitionId, Type>();
        }

        public void RegisterTransition(TransitionId transitionId, Type targetStateType)
        {
            this._transitionDictionary.Add(transitionId, targetStateType);
        }

        public Type GetTargetTypeForTransition(TransitionId transitionId)
        {
            return this._transitionDictionary.ContainsKey(transitionId) ? this._transitionDictionary[transitionId] : null;
        }
    }
}
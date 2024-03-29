﻿namespace Omnipaste.Framework.Services.ActivationServiceData
{
    using System;
    using System.Collections.Generic;
    using Omnipaste.Framework.Services.ActivationServiceData.Transitions;

    public class ActivationSequence
    {
        public Type InitialStepId { get; set; }

        public TransitionCollection Transitions { get; set; }

        public List<object> FinalStepIdIds { get; set; }

        public ActivationSequence()
        {
            FinalStepIdIds = new List<object>();
        }
    }
}
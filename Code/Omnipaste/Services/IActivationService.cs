﻿namespace Omnipaste.Services
{
    using System;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    public interface IActivationService
    {
        #region Public Properties

        IActivationStep CurrentStep { get; }

        bool Success { get; }

        #endregion

        #region Public Methods and Operators

        IObservable<IActivationStep> Run();

        #endregion
    }
}
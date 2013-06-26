﻿namespace OmniCommon.Services.ActivationServiceData.ActivationServiceSteps
{
    public interface IActivationStep
    {
        DependencyParameter Parameter { get; set; }

        IExecuteResult Execute();

        object GetId();
    }
}
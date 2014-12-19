﻿namespace OmniUI.Services
{
    using System;
    using OmniUI.Framework.Commands;

    public interface ICommandService
    {
        IObservable<TResult> Execute<TResult>(ICommand<TResult> command);
    }
}
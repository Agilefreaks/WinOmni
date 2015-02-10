namespace Omnipaste.Services.Commands
{
    using System;
    using System.Reactive.Linq;
    using Ninject;
    using OmniUI.Framework.Commands;
    using OmniUI.Services;

    public class CommandService : ICommandService
    {
        #region Fields

        private readonly IKernel _kernel;

        #endregion

        #region Constructors and Destructors

        public CommandService(IKernel kernel)
        {
            _kernel = kernel;
        }

        #endregion

        #region Public Methods and Operators

        public IObservable<TResult> Execute<TResult>(IObservableCommand<TResult> command)
        {
            _kernel.Inject(command);

            return Observable.Defer(command.Execute);
        }

        #endregion
    }
}
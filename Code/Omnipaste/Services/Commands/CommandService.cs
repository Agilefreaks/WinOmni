namespace Omnipaste.Services.Commands
{
    using System;
    using System.Reactive.Linq;
    using Ninject;

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

        public IObservable<TResult> Execute<TCommand, TResult>(TCommand command)
        {
            var commandProcessor = _kernel.Get<ICommandProcessor<TCommand, TResult>>();

            return Observable.Defer(() => commandProcessor.Process(command));
        }

        #endregion
    }
}
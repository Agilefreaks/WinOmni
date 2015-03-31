namespace Omnipaste.Framework.Services.Commands
{
    using System;
    using Ninject;
    using OmniUI.Framework.Commands;
    using OmniUI.Framework.Services;

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

            return command.Execute();
        }

        #endregion
    }
}
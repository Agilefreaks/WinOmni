namespace Omnipaste.Services
{
    using System.Threading.Tasks;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;
    using Omnipaste.Services.ActivationServiceData.Transitions;

    public interface IActivationService
    {
        #region Public Properties

        IActivationStep CurrentStep { get; }

        bool Success { get; }

        TransitionCollection Transitions { get; }

        #endregion

        #region Public Methods and Operators

        Task Run();

        #endregion
    }
}
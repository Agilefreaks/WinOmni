namespace Omnipaste.Services
{
    using System.Threading.Tasks;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    public interface IActivationService
    {
        #region Public Properties

        IActivationStep CurrentStep { get; }

        #endregion

        #region Public Methods and Operators

        Task Run();

        #endregion
    }
}
namespace Omnipaste.Loading.ActivationFailed
{
    using Caliburn.Micro;
    using Ninject;
    using Omnipaste.Framework;

    public interface IActivationFailedViewModel : IScreen
    {
        void Retry();

        void Exit();

        IApplicationService ApplicationService { get; set; }

        [Inject]
        IEventAggregator EventAggregator { get; set; }
    }
}
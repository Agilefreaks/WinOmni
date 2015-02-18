namespace Omnipaste.ClippingList
{
    using Microsoft.Practices.ServiceLocation;
    using Omnipaste.Presenters;

    public class ClippingViewModelFactory : IClippingViewModelFactory
    {
        private readonly IServiceLocator _serviceLocator;

        public ClippingViewModelFactory(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public IClippingViewModel Create(ClippingPresenter clippingPresenter)
        {
            var viewModel = _serviceLocator.GetInstance<IClippingViewModel>();
            viewModel.Model = clippingPresenter;

            return viewModel;
        }
    }
}
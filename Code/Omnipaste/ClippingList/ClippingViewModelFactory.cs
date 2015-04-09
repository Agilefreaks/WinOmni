namespace Omnipaste.ClippingList
{
    using Microsoft.Practices.ServiceLocation;
    using Omnipaste.ClippingList.Clipping;
    using Omnipaste.Models;

    public class ClippingViewModelFactory : IClippingViewModelFactory
    {
        private readonly IServiceLocator _serviceLocator;

        public ClippingViewModelFactory(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public IClippingViewModel Create(ClippingModel clippingModel)
        {
            var viewModel = _serviceLocator.GetInstance<IClippingViewModel>();
            viewModel.Model = clippingModel;

            return viewModel;
        }
    }
}
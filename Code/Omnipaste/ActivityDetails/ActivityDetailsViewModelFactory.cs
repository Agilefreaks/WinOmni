namespace Omnipaste.ActivityDetails
{
    using Activity.Models;
    using Ninject;
    using Omnipaste.ActivityDetails.Clipping;
    using Omnipaste.ActivityDetails.Message;

    public class ActivityDetailsViewModelFactory : IActivityDetailsViewModelFactory
    {
        [Inject]
        public IKernel Kernel { get; set; }

        public IActivityDetailsViewModel Create(Activity activity)
        {
            IActivityDetailsViewModel result;
            switch (activity.Type)
            {
                case ActivityTypeEnum.Clipping:
                    result = Kernel.Get<IClippingDetailsViewModel>();
                    break;
                case ActivityTypeEnum.Message:
                    result = Kernel.Get<IMessageDetailsViewModel>();
                    break;
                default:
                    result = new ActivityDetailsViewModel(
                        new ActivityDetailsHeaderViewModel(),
                        new ActivityDetailsContentViewModel());
                    break;
            }

            result.HeaderViewModel.Model = activity;
            result.ContentViewModel.Model = activity;

            return result;
        }
    }
}
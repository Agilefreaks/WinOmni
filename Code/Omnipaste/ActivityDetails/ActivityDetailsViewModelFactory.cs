namespace Omnipaste.ActivityDetails
{
    using Ninject;
    using Omnipaste.ActivityDetails.Clipping;
    using Omnipaste.ActivityDetails.Conversation;
    using Omnipaste.ActivityDetails.Version;
    using Omnipaste.Models;

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
                case ActivityTypeEnum.Call:
                    result = Kernel.Get<IConversationViewModel>();
                    break;
                case ActivityTypeEnum.Version:
                    result = Kernel.Get<IVersionDetailsViewModel>();
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
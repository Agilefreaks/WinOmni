namespace Omnipaste.ActivityDetails
{
    using Activity.Models;
    using Omnipaste.ActivityDetails.Clipping;
    using Omnipaste.ActivityDetails.Message;

    public class ActivityDetailsViewModelFactory : IActivityDetailsViewModelFactory
    {
        public IActivityDetailsViewModel Create(Activity activity)
        {
            IActivityDetailsViewModel result;
            switch (activity.Type)
            {
                case ActivityTypeEnum.Clipping:
                    result = new ClippingDetailsViewModel(
                        new ClippingDetailsHeaderViewModel(),
                        new ClippingDetailsContentViewModel());
                    break;
                case ActivityTypeEnum.Message:
                    result = new MessageDetailsViewModel(
                        new MessageDetailsHeaderViewModel(),
                        new ActivityDetailsContentViewModel());
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
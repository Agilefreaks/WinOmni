namespace Omnipaste.ActivityList
{
    using Caliburn.Micro;

    public class ActivityListViewModel : Screen, IActivityListViewModel
    {
        public override string DisplayName
        {
            get
            {
                return Properties.Resources.Activity;
            }
        }
    }
}
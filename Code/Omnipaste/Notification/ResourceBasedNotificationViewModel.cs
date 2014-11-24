namespace Omnipaste.Notification
{
    public abstract class ResourceBasedNotificationViewModel<T> : NotificationViewModelBase, IResourceBasedNotificationViewModel<T>
    {
        private T _resource;

        public virtual T Resource
        {
            get
            {
                return _resource;
            }
            set
            {
                if (Equals(value, _resource))
                {
                    return;
                }
                _resource = value;
                NotifyOfPropertyChange(() => Resource);
                NotifyOfPropertyChange(() => Line1);
                NotifyOfPropertyChange(() => Line2);
                NotifyOfPropertyChange(() => Title);
            }
        }

    }
}
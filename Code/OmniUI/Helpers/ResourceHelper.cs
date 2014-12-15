namespace OmniUI.Helpers
{
    public class ResourceHelper
    {
        public static object GetByKey(string key)
        {
            return ApplicationHelper.Instance.FindResource(key);
        }

        public static TResource GetByKey<TResource>(string key)
        {
            return (TResource)GetByKey(key);
        }
    }
}

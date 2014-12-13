namespace OmniUI.Helpers
{
    using System.Windows;

    public class ResourceHelper
    {
        public static object GetByKey(string key)
        {
            return Application.Current.FindResource(key);
        }

        public static TResource GetByKey<TResource>(string key)
        {
            return (TResource)GetByKey(key);
        }
    }
}

namespace Omnipaste.Services
{
    public class SessionItemChangeEventArgs
    {
        public string Key { get; private set; }

        public object OldValue { get; private set; }

        public object NewValue { get; private set; }

        public SessionItemChangeEventArgs(string key, object newValue, object oldValue)
        {
            Key = key;
            NewValue = newValue;
            OldValue = oldValue;
        }
    }
}
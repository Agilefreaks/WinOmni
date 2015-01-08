namespace Omnipaste.EventAggregatorMessages
{
    public class DismissNotification
    {
        public object Identifier { get; set; }

        public DismissNotification()
        {
        }

        public DismissNotification(object identifier)
        {
            Identifier = identifier;
        }
    }
}

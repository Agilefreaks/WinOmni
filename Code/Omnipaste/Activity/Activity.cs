namespace Omnipaste.Activity
{
    using Clipboard.Models;
    using Events.Models;

    public class Activity
    {
        public string Content { get; private set; }

        public Activity(Clipping clipping)
        {
            Content = clipping.Content;
        }

        public Activity(Event @event)
        {
            Content = @event.Content;
        }
    }
}
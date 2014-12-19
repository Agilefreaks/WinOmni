namespace OmniHolidays.Providers
{
    public class MessageDefinition
    {
        public string Language { get; set; }

        public string Category { get; set; }

        public string MessageTemplate { get; set; }

        public MessageDefinition(string language, string category, string messageTemplate)
        {
            Language = language;
            Category = category;
            MessageTemplate = messageTemplate;
        }
    }
}
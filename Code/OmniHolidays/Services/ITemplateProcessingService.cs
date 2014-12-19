namespace OmniHolidays.Services
{
    using OmniUI.Models;

    public interface ITemplateProcessingService
    {
        string Process(string template, IContactInfo contactInfo);
    }
}
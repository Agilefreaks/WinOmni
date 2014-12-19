namespace OmniHolidays
{
    using Caliburn.Micro;
    using OmniUI.Presenters;

    public class ContactInfoSelectionSource
    {
        public static IObservableCollection<IContactInfoPresenter> Current { get; set; }
    }
}
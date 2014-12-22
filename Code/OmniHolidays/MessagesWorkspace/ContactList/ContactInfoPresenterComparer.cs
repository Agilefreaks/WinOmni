namespace OmniHolidays.MessagesWorkspace.ContactList
{
    using System;
    using System.Collections;
    using OmniUI.Presenters;

    public class ContactInfoPresenterComparer : IComparer
    {
        private readonly StringComparer _stringComparer;

        public ContactInfoPresenterComparer()
        {
            _stringComparer = StringComparer.CurrentCultureIgnoreCase;
        }

        public int Compare(object x, object y)
        {
            var model1 = (IContactInfoPresenter)x;
            var model2 = (IContactInfoPresenter)y;

            return _stringComparer.Compare(model1.Identifier, model2.Identifier);
        }
    }
}
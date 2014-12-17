namespace OmniHolidays.MessagesWorkspace.ContactList
{
    using System;
    using System.Collections;

    public class ContactViewModelComparer : IComparer
    {
        private readonly StringComparer _stringComparer;

        public ContactViewModelComparer()
        {
            _stringComparer = StringComparer.CurrentCultureIgnoreCase;
        }

        public int Compare(object x, object y)
        {
            var viewModel1 = (IContactViewModel)x;
            var viewModel2 = (IContactViewModel)y;

            return _stringComparer.Compare(viewModel1.Model.Identifier, viewModel2.Model.Identifier);
        }
    }
}
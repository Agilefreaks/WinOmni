namespace Omnipaste.Presenters
{
    using System.Collections.Generic;
    using Caliburn.Micro;

    public class GroupMessagePresenter : PropertyChangedBase, IGroupMessagePresenter
    {
        private string _message;

        private List<ContactInfoPresenter> _selectedContacts;

        public List<ContactInfoPresenter> SelectedContacts
        {
            get
            {
                return _selectedContacts;
            }
            set
            {
                if (value != _selectedContacts)
                {
                    _selectedContacts = value;
                    NotifyOfPropertyChange(() => SelectedContacts);
                }
            }
        }

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                if (value != _message)
                {
                    _message = value;
                    NotifyOfPropertyChange(() => Message);
                }
            }
        }

        public GroupMessagePresenter()
        {
            SelectedContacts = new List<ContactInfoPresenter>();
        }
    }
}
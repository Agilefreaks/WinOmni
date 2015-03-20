namespace Omnipaste.GroupMessage.GroupMessageDetails{
    using System.Collections.ObjectModel;
    using Caliburn.Micro;
    using Contacts.Models;
    using Omnipaste.Controls;
    using Omnipaste.ExtensionMethods;
    using Omnipaste.Models;
    using Omnipaste.Presenters;

    public class GroupMessageHeaderViewModel : Screen, IGroupMessageHeaderViewModel
    {
        private ObservableCollection<ContactInfoPresenter> _recipients;

        public ObservableCollection<ContactInfoPresenter> Recipients
        {
            get
            {
                return _recipients;
            }
            set
            {
                if (Equals(value, _recipients))
                {
                    return;
                }

                _recipients = value;
                NotifyOfPropertyChange(() => Recipients);
            }
        }

        public void AddNewContact(TokenEventArgs args)
        {
            var contactDto = new ContactDto();
            contactDto.PhoneNumbers.Add(new PhoneNumberDto { Number = args.TokenIdentifier });
            Recipients.Add(new ContactInfoPresenter(new ContactInfo(contactDto)));
        }

        public void RemoveContact(ContactInfoPresenter itemToRemove)
        {
            Recipients.Remove(itemToRemove);
        }
    }
}
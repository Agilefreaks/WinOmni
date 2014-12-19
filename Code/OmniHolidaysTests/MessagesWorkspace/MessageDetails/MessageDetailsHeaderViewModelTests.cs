namespace OmniHolidaysTests.MessagesWorkspace.MessageDetails
{
    using System.Linq;
    using Caliburn.Micro;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniHolidays.MessagesWorkspace.ContactList;
    using OmniHolidays.MessagesWorkspace.MessageDetails;
    using OmniUI.Framework;
    using OmniUI.Presenters;

    [TestFixture]
    public class MessageDetailsHeaderViewModelTests
    {
        private MessageDetailsHeaderViewModel _subject;

        private Mock<IContactSource> _mockContactsSource;

        [SetUp]
        public void Setup()
        {
            _mockContactsSource = new Mock<IContactSource>();
            _subject = new MessageDetailsHeaderViewModel();
        }

        [Test]
        public void ClearContacts_Always_SetsIsSelectedFalseForAllCurrentContacts()
        {
            var contactViewModels = Enumerable.Range(0, 2).Select(_ => GetContactViewModel()).ToList();
            var collectionView = new DeepObservableCollectionView<ContactViewModel>(
                new BindableCollection<ContactViewModel>(contactViewModels));
            collectionView.Filter = item => ((ContactViewModel)item).IsSelected;
            _mockContactsSource.Setup(x => x.Contacts).Returns(collectionView);
            _subject.ContactsSource = _mockContactsSource.Object;
            _subject.ContactsSource.Contacts.Cast<object>().Count().Should().Be(2);

            _subject.ClearContacts();

            _subject.ContactsSource.Contacts.Should().BeEmpty();
        }

        private static ContactViewModel GetContactViewModel()
        {
            return new ContactViewModel { Model = new Mock<IContactInfoPresenter>().Object, IsSelected = true };
        }
    }
}
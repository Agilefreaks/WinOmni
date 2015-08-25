namespace OmnipasteTests.Conversations.Conversation
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Conversations.Conversation;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.Models;

    [TestFixture]
    public class RecepientsTokenizerTests
    {            
        [Test]
        public void Tokenize_WithSimpleString_WillAddOneContact()
        {
            var recipients = new ObservableCollection<ContactModel>();
            IRecepientsTokenizer subject = new RecepientsTokenizer(recipients);

            subject.Tokenize("42");

            recipients.Count.Should().Be(1);
            recipients.First().PhoneNumber.Should().Be("42");
        }

        [Test]
        public void Tokenize_WithStringWithSeparator_WillAddContacts()
        {
            var recipients = new ObservableCollection<ContactModel>();
            IRecepientsTokenizer subject = new RecepientsTokenizer(recipients);

            subject.Tokenize("42;43");

            recipients.Count.Should().Be(2);
            recipients.First().PhoneNumber.Should().Be("42");
            recipients.Last().PhoneNumber.Should().Be("43");
        }

        [Test]
        public void Tokenize_WithStringWithSeparatorAndSpace_WillAddContacts()
        {
            var recipients = new ObservableCollection<ContactModel>();
            IRecepientsTokenizer subject = new RecepientsTokenizer(recipients);

            subject.Tokenize("42; 43");

            recipients.Count.Should().Be(2);
            recipients.First().PhoneNumber.Should().Be("42");
            recipients.Last().PhoneNumber.Should().Be("43");
        }

        [Test]
        public void Tokenize_WithAnElementThatsAlreadyInRecepients_WillNotAddItAgain()
        {
            var recipients = new ObservableCollection<ContactModel> { BuildContactModel() };
            IRecepientsTokenizer subject = new RecepientsTokenizer(recipients);

            subject.Tokenize("42; 43");

            recipients.Count.Should().Be(2);
            recipients.First().PhoneNumber.Should().Be("42");
            recipients.Last().PhoneNumber.Should().Be("43");
        }

        [Test]
        public void Tokenize_WhenStringIsEmpty_ShouldNotAddARecipient()
        {
            var recipients = new ObservableCollection<ContactModel>();
            IRecepientsTokenizer subject = new RecepientsTokenizer(recipients);

            subject.Tokenize("");

            recipients.Should().BeEmpty();
        }

        [Test]
        public void Tokenize_WhenCallingTokenizer_RecepientsShouldBeEmpty()
        {
            var recipients = new ObservableCollection<ContactModel>();
            IRecepientsTokenizer subject = new RecepientsTokenizer(recipients);

            subject.Tokenize("43; 44");
            subject.Tokenize("42");

            recipients.Count.Should().Be(1);
            recipients.First().PhoneNumber.Should().Be("42");
        }

        [Test]
        public void Tokenize_WhenNoRecepients_WillReturnAnEmptyString()
        {
            var recipients = new ObservableCollection<ContactModel>();
            IRecepientsTokenizer subject = new RecepientsTokenizer(recipients);

            // Todo: fix the assertion
            subject.Tokenize().Should().BeEmpty();
        }

        [Test]
        public void Tokenize_WithARecepient_WillReturnTheStringWithThePhoneNumber()
        {
            var recipients = new ObservableCollection<ContactModel> { BuildContactModel() };
            IRecepientsTokenizer subject = new RecepientsTokenizer(recipients);

            // Todo: fix the assertion
            subject.Tokenize().Should().Be("42");
        }

        [Test]
        public void Tokenize_WithMultipleRecipients_WillReturnTheStringWithPhoneNumbers()
        {
            var recipients = new ObservableCollection<ContactModel> { BuildContactModel(), BuildContactModel("43") };
            IRecepientsTokenizer subject = new RecepientsTokenizer(recipients);

            // Todo: add assertion
        }

        private static ContactModel BuildContactModel(string phoneNumber = "42")
        {
            return new ContactModel(new ContactEntity() { PhoneNumbers = new List<PhoneNumber> { new PhoneNumber(phoneNumber) } });
        }
    }
}
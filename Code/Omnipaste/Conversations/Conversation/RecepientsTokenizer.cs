namespace Omnipaste.Conversations.Conversation
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Castle.Core.Internal;
    using Framework.Entities;
    using Framework.Models;

    public class RecepientsTokenizer : IRecepientsTokenizer
    {
        private readonly ObservableCollection<ContactModel> _recipients;

        public RecepientsTokenizer(ObservableCollection<ContactModel> recipients)
        {
            _recipients = recipients;
        }

        public void Tokenize(string tokens)
        {
           _recipients.Clear();

            if (string.IsNullOrEmpty(tokens))
            {
                return;
            }

            var splitTokens = tokens.Split(new[] { ";" }, StringSplitOptions.None);

            splitTokens.ForEach(
                token =>
                    {
                        var trimmedToken = token.Trim();
                        if (!RecipientAlreadyAdded(trimmedToken))
                        {
                            _recipients.Add(BuildContactModel(trimmedToken));
                        }
                    });
        }

        public string Tokenize()
        {
            return string.Join("; ", _recipients.Select(recipient => recipient.PhoneNumber));
        }

        private bool RecipientAlreadyAdded(string number)
        {
            return _recipients.Any(contactModel => contactModel.PhoneNumber.Equals(number));
        }

        private ContactModel BuildContactModel(string number)
        {
            return
                new ContactModel(
                    new ContactEntity() { PhoneNumbers = new List<PhoneNumber> { new PhoneNumber(number) } });
        }
    }
}
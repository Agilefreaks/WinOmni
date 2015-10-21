namespace Omnipaste.Conversations.Conversation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Framework.Entities;
    using Framework.Models;
    using OmniUI.Controls;

    public class RecepientsTokenizer : IRecepientsTokenizer
    {
        public const string TokenSeparator = " ";

        public ITokenizeResult Tokenize(string text)
        {
            var result = new TokenizeResult { NonTokenizedText = text };
            if (string.IsNullOrEmpty(text) || !text.Contains(TokenSeparator))
            {
                return result;
            }

            var tokens = new List<ContactModel>();
            while (result.NonTokenizedText.Contains(TokenSeparator))
            {
                var tokenEndIndex = result.NonTokenizedText.IndexOf(TokenSeparator, StringComparison.Ordinal);
                var tokenText = result.NonTokenizedText.Substring(0, tokenEndIndex);
                var trimmedText = tokenText.Trim();
                if(trimmedText != string.Empty && tokens.All(token => token.PhoneNumber != trimmedText))
                {
                    tokens.Add(BuildContactModel(trimmedText));
                }

                result.NonTokenizedText = result.NonTokenizedText.Substring(tokenEndIndex + 1);
            }

            result.Tokens = tokens;

            return result;
        }

        private static ContactModel BuildContactModel(string number)
        {
            return
                new ContactModel(
                    new ContactEntity() { PhoneNumbers = new List<PhoneNumber> { new PhoneNumber(number) } });
        }
    }
}
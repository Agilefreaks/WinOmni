namespace Omnipaste.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class TextParser
    {
        private List<Tuple<TextPartTypeEnum, string>> _result;

        public List<Tuple<TextPartTypeEnum, string>> Parse(string text)
        {
            _result = new List<Tuple<TextPartTypeEnum, string>>();
            int previousPlainTextPartStartIndex;
            int previousPlainTextPartEndIndex;
            var regularExpression =
                new Regex(
                    @"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)",
                    RegexOptions.IgnoreCase);
            var matches = regularExpression.Matches(text);
            for (var index = 0; index < matches.Count; index++)
            {
                var match = matches[index];
                previousPlainTextPartStartIndex = index > 0 ? matches[index - 1].Index + matches[index - 1].Length : 0;
                previousPlainTextPartEndIndex = match.Index;
                AddPlainTextPart(text, previousPlainTextPartStartIndex, previousPlainTextPartEndIndex);
                _result.Add(new Tuple<TextPartTypeEnum, string>(TextPartTypeEnum.Hyperlink, match.Captures[0].Value));
            }

            previousPlainTextPartStartIndex = matches.Count > 0
                               ? matches[matches.Count - 1].Index + matches[matches.Count - 1].Length
                               : 0;
            previousPlainTextPartEndIndex = text.Length - previousPlainTextPartStartIndex;
            AddPlainTextPart(text, previousPlainTextPartStartIndex, previousPlainTextPartEndIndex);

            return _result;
        }

        private void AddPlainTextPart(string text, int startIndex, int endIndex)
        {
            var previousTextPart = text.Substring(startIndex, endIndex);
            if (!string.IsNullOrWhiteSpace(previousTextPart))
            {
                _result.Add(new Tuple<TextPartTypeEnum, string>(TextPartTypeEnum.PlainText, previousTextPart));
            }
        }
    }
}
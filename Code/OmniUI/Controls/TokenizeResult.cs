namespace OmniUI.Controls
{
    using System.Collections.Generic;

    public class TokenizeResult : ITokenizeResult
    {
        public IEnumerable<object> Tokens { get; set; }

        public string NonTokenizedText { get; set; }

        public TokenizeResult()
        {
            Tokens = new object[0];
            NonTokenizedText = string.Empty;
        }
    }
}
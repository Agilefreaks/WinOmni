namespace OmniUI.Controls
{
    using System.Collections.Generic;

    public interface ITokenizeResult
    {
        IEnumerable<object> Tokens { get; }

        string NonTokenizedText { get; }
    }
}
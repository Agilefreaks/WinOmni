namespace OmniApi.Support.Serialization
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using Newtonsoft.Json.Serialization;

    public class DeliminatorSeparatedPropertyNamesContractResolver : DefaultContractResolver
    {
        #region Fields

        private readonly string _separator;

        #endregion

        #region Constructors and Destructors

        protected DeliminatorSeparatedPropertyNamesContractResolver(char separator)
            : base(true)
        {
            _separator = separator.ToString(CultureInfo.InvariantCulture);
        }

        #endregion

        #region Methods

        protected override string ResolvePropertyName(string propertyName)
        {
            var parts = new List<string>();
            var currentWord = new StringBuilder();

            foreach (var c in propertyName)
            {
                if (char.IsUpper(c) && currentWord.Length > 0)
                {
                    parts.Add(currentWord.ToString());
                    currentWord.Clear();
                }
                currentWord.Append(char.ToLower(c));
            }

            if (currentWord.Length > 0)
            {
                parts.Add(currentWord.ToString());
            }

            return string.Join(_separator, parts.ToArray());
        }

        #endregion
    }
}
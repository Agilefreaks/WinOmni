namespace InstallerCustomActions
{
    using System.IO;
    using System.Text.RegularExpressions;

    public class AuthorizationBootstrapper
    {
        private static readonly Regex AuthorizationKeyRegex = new Regex(@"^(.*)(\d{6})$");

        public static string ExtractAuthorizationKey(string msiFileName)
        {
            var authorizationKey = string.Empty;

            if (msiFileName == null) return authorizationKey;

            var fileName = Path.GetFileNameWithoutExtension(msiFileName);
            var matchCollection = AuthorizationKeyRegex.Matches(fileName);
            if (matchCollection.Count > 0)
            {
                authorizationKey = matchCollection[0].Groups[2].Captures[0].Value;
            }

            return authorizationKey;
        }
    }
}
using Microsoft.Deployment.WindowsInstaller;

namespace InstallerCustomActions
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text.RegularExpressions;

    public class CustomActions
    {
        private static readonly Regex AuthorizationKeyRegex = new Regex(@"^(.*)(\d{6})$");

        [CustomAction]
        public static ActionResult StartAppWithAuthorizationKey(Session session)
        {
            var result = ActionResult.Success;
            try
            {
                StartApp(ExtractAuthorizationKey(session), session);
            }
            catch (Exception exception)
            {
                session.Log(exception.ToString());
                result = ActionResult.Failure;
            }

            return result;
        }

        private static void StartApp(string authorizationKey, Session session)
        {
            var targetDir = session.CustomActionData["TargetDir"];
            var target = session.CustomActionData["Target"];
            var targetPath = Path.Combine(targetDir, target);

            var arguments = string.Format("-authorizationKey={0}", authorizationKey);
            Process.Start(new ProcessStartInfo(targetPath) { Arguments = arguments, WorkingDirectory = targetDir, });
        }

        private static string ExtractAuthorizationKey(Session session)
        {
            var authorizationKey = string.Empty;
            var msiFileName = session.CustomActionData["OriginalDatabase"];

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

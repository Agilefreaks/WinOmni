namespace InstallerCustomActions
{
    using System;
    using Microsoft.Deployment.WindowsInstaller;

    public class CustomActions
    {
        [CustomAction]
        public static ActionResult SetStartArguments(Session session)
        {
            var result = ActionResult.Success;
            try
            {
                var msiFileName = session["OriginalDatabase"];
                var authorizationKey = AuthorizationBootstrapper.ExtractAuthorizationKey(msiFileName);
                var arguments = string.IsNullOrWhiteSpace(authorizationKey)
                                    ? " -updated"
                                    : string.Format("-authorizationKey={0}", authorizationKey);

                if (session["START_MINIMIZED"] == "true")
                {
                    arguments += " -minimized";
                }

                session["STARTAPPARGUMENTS"] = arguments;
            }
            catch (Exception exception)
            {
                session.Log(exception.ToString());
                result = ActionResult.Failure;
            }

            return result;
        }
    }
}

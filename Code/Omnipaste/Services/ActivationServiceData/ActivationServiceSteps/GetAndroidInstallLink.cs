namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Web;
    using OmniApi.Models;

    public class GetAndroidInstallLink : SynchronousStepBase
    {
        private const string UrlFormat = "{0}?email={1}";

        private const string BaseUrl = "https://www.omnipasteapp.com/downloads/android_client";

        protected override IExecuteResult ExecuteSynchronously()
        {
            var userInfo = Parameter.Value as UserInfo ?? new UserInfo();
            var safeEmailString = HttpUtility.UrlEncode(userInfo.Email);
            return new ExecuteResult(
                SimpleStepStateEnum.Successful,
                new Uri(string.Format(UrlFormat, BaseUrl, safeEmailString)));
        }
    }
}
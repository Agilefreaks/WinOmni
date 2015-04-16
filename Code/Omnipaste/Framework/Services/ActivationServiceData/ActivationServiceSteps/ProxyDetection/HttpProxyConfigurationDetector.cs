namespace Omnipaste.Framework.Services.ActivationServiceData.ActivationServiceSteps.ProxyDetection
{
    using OmniCommon.Interfaces;
    using OmniCommon.Models;

    public class HttpProxyConfigurationDetector : SystemWebProxyConfigurationDetector
    {
        #region Constructors and Destructors

        public HttpProxyConfigurationDetector(IConfigurationService configurationService)
            : base(configurationService)
        {
        }

        #endregion

        #region Public Properties

        public override ProxyTypeEnum ProxyType
        {
            get
            {
                return ProxyTypeEnum.Http;
            }
        }

        #endregion
    }
}
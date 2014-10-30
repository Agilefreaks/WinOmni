namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps.ProxyDetection
{
    using OmniCommon;
    using OmniCommon.Interfaces;

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
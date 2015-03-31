namespace Omnipaste.Framework.Services.ActivationServiceData.ActivationServiceSteps.ProxyDetection
{
    using OmniCommon.Interfaces;
    using OmniCommon.Models;

    public class SocksProxyConfigurationDetector : SystemWebProxyConfigurationDetector
    {
        #region Constructors and Destructors

        public SocksProxyConfigurationDetector(IConfigurationService configurationService)
            : base(configurationService)
        {
        }

        #endregion

        #region Public Properties

        public override ProxyTypeEnum ProxyType
        {
            get
            {
                return ProxyTypeEnum.Socks4;
            }
        }

        #endregion

        #region Methods

        protected override string GetPingEndpoint()
        {
            var pingEndpoint = base.GetPingEndpoint();
            var endpointWithSocks = pingEndpoint.Replace("https", "socks").Replace("http", "socks");

            return endpointWithSocks;
        }

        #endregion
    }
}
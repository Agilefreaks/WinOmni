namespace Clipboard.Handlers
{
    using System;
    using System.Reactive.Linq;
    using Clipboard.API.Resources.v1;
    using Clipboard.Models;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Handlers;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;

    public class ClippingCreatedHandler : ResourceHandler<Clipping>, IOmniClipboardHandler
    {
        private readonly IClippings _clippingsResource;

        public override string HandledMessageType
        {
            get
            {
                return "clipping_created";
            }
        }
        
        public ClippingCreatedHandler(IClippings clippingsResource, IConfigurationService configurationService)
        {
            _clippingsResource = clippingsResource;
            ConfigurationService = configurationService;
        }

        public IConfigurationService ConfigurationService { get; set; }

        public IObservable<Clipping> Clippings
        {
            get
            {
                return InnerSubject;
            }
        }

        public void PostClipping(Clipping clipping)
        {
            _clippingsResource.Create(ConfigurationService.DeviceId, clipping.Content).RunToCompletion();
        }

        protected override IObservable<Clipping> CreateResult(OmniMessage value)
        {
            var clippingId = value.GetPayload("id");
            return _clippingsResource.Get(clippingId).Select(
                clipping =>
                {
                    clipping.Source = Clipping.ClippingSourceEnum.Cloud;
                    return clipping;
                });
        }
    }
}
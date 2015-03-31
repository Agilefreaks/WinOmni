namespace Clipboard.Handlers
{
    using System;
    using System.Reactive.Linq;
    using Clipboard.API.Resources.v1;
    using Clipboard.Dto;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Handlers;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;

    public class ClippingCreatedHandler : ResourceHandler<ClippingDto>, IOmniClipboardHandler
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

        public IObservable<ClippingDto> Clippings
        {
            get
            {
                return InnerSubject;
            }
        }

        public void PostClipping(ClippingDto clippingDto)
        {
            _clippingsResource.Create(ConfigurationService.DeviceId, clippingDto.Content).RunToCompletion();
        }

        protected override IObservable<ClippingDto> CreateResult(OmniMessage value)
        {
            var clippingId = value.GetPayload("id");
            return _clippingsResource.Get(clippingId).Select(
                clipping =>
                {
                    clipping.Source = ClippingDto.ClippingSourceEnum.Cloud;
                    return clipping;
                });
        }
    }
}
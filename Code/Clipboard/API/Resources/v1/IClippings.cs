namespace Clipboard.API.Resources.v1
{
    using System;
    using Clipboard.Dto;
    using OmniCommon.Interfaces;

    public interface IClippings : IResource<ClippingDto>
    {
        #region Public Methods and Operators

        IObservable<ClippingDto> Create(string deviceId, string content);

        #endregion
    }
}
namespace Clipboard.API.Resources.v1
{
    using System;
    using Clipboard.Models;
    using OmniCommon.Interfaces;

    public interface IClippings : IResource<Clipping>
    {
        #region Public Methods and Operators

        IObservable<Clipping> Create(string deviceId, string content);

        #endregion
    }
}
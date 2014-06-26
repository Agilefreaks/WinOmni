namespace Clipboard.API.Resources.v1
{
    using System;
    using Clipboard.Models;

    public interface IClippings
    {
        #region Public Methods and Operators

        IObservable<Clipping> Create(string identifier, string content);

        IObservable<Clipping> Last();

        #endregion
    }
}
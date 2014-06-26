namespace Omnipaste.Clipping
{
    using Caliburn.Micro;
    using Clipboard.Models;

    public interface IClippingViewModel : IScreen
    {
        Clipping Model { get; set; }
    }
}
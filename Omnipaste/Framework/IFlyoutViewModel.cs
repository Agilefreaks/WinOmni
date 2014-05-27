using Caliburn.Micro;
using MahApps.Metro.Controls;

namespace Omnipaste.Framework
{
    public interface IFlyoutViewModel : IScreen
    {
        string Header { get; set; }

        bool IsOpen { get; set; }

        bool IsModal { get; set; }
        bool IsPinned { get; set; }

        Position Position { get; set; }
    }
}
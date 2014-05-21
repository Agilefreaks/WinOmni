using MahApps.Metro.Controls;

namespace Omnipaste.UserToken
{
    public interface IFlyoutViewModel
    {
        string Header { get; set; }

        bool IsOpen { get; set; }

        bool IsModal { get; set; }

        Position Position { get; set; }
    }
}
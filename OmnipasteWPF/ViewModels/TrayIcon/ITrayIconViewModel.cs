namespace OmnipasteWPF.ViewModels.TrayIcon
{
    public interface ITrayIconViewModel
    {
        bool TrayIconVisible { get; set; }

        void Start();
    }
}
namespace OmnipasteWPF.ViewModels.TrayIcon
{
    using System.ComponentModel;
    using Cinch;

    public class TrayIconViewModel : ViewModel, ITrayIconViewModel
    {
        private static readonly PropertyChangedEventArgs TrayIconVisibleChangeArgs =
            ObservableHelper.CreateArgs<TrayIconViewModel>(x => x.TrayIconVisible);

        private bool _trayIconVisible;

        public bool TrayIconVisible
        {
            get
            {
                return this._trayIconVisible;
            }

            set
            {
                this._trayIconVisible = value;
                this.NotifyPropertyChanged(TrayIconVisibleChangeArgs);
            }
        }

        public TrayIconViewModel(IIOCProvider iocProvider)
            : base(iocProvider)
        {
        }
    }
}
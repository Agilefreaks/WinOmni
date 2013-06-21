namespace OmnipasteWPF.ViewModels
{
    using System.ComponentModel;
    using Cinch;

    public class TrayIconViewModel : ViewModelBase, ITrayIconViewModel
    {
        private static readonly PropertyChangedEventArgs TrayIconVisibleChangeArgs =
            ObservableHelper.CreateArgs<TrayIconViewModel>(x => x.TrayIconVisible);

        private bool _trayIconVisible;

        public bool TrayIconVisible
        {
            get
            {
                return _trayIconVisible;
            }

            set
            {
                _trayIconVisible = value;
                NotifyPropertyChanged(TrayIconVisibleChangeArgs);
            }
        }

        public TrayIconViewModel(IIOCProvider iocProvider)
            : base(iocProvider)
        {
        }
    }
}
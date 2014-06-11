namespace Omnipaste.Shell
{
    using Caliburn.Micro;
    using Omnipaste.Dialog;
    using Omnipaste.Loading;

    public interface IShellViewModel : IConductor, IScreen, IViewAware
    {
        ILoadingViewModel LoadingViewModel { get; set; }

        IDialogViewModel DialogViewModel { get; set; }
    }
}
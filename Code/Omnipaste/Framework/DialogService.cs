namespace Omnipaste.Framework
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using Caliburn.Micro;
    using MahApps.Metro.Controls;
    using MahApps.Metro.Controls.Dialogs;
    using Omnipaste.Dialog;

    public class DialogService : IDialogService
    {
        private BaseMetroDialog _dialog;

        private DialogViewModel _dialogViewModel;

        public async Task ShowDialog(IScreen viewModel)
        {
            _dialogViewModel = IoC.Get<DialogViewModel>();
            _dialogViewModel.Content = viewModel;

            viewModel.Deactivated += ViewModelDeactivated;

            var view = ViewLocator.GetViewForViewModel(_dialogViewModel);
            _dialog = view as BaseMetroDialog;
                    
            if (_dialog == null)
            {
                throw new InvalidOperationException(
                    String.Format(
                        "The view {0} belonging to view model {1} does not inherit from {2}",
                        view.GetType(),
                        viewModel.GetType(),
                        typeof(BaseMetroDialog)));
            }

            var shellView = Application.Current.Windows.OfType<MetroWindow>().First();
                    
            await shellView.ShowMetroDialogAsync(_dialog);
        }

        private void ViewModelDeactivated(object sender, DeactivationEventArgs e)
        {
            var screen = (IDeactivate)sender;
            screen.Deactivated -= ViewModelDeactivated;
            _dialogViewModel.DeactivateItem(screen, true);
            var firstMetroWindow = Application.Current.Windows.OfType<MetroWindow>().First();

            Execute.OnUIThread(async () => await firstMetroWindow.HideMetroDialogAsync(_dialog));
        }
    }
}
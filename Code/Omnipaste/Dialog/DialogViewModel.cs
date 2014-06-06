namespace Omnipaste.Dialog
{
    using System;
    using Caliburn.Micro;
    using Ninject;
    using Omnipaste.Framework;

    public class DialogClosedEventArgs : EventArgs
    {
        public object ClosedItem { get; set; }
    }

    public class DialogViewModel : Conductor<IScreen>, IDialogViewModel
    {
        #region Fields

        private bool _isOpen;

        #endregion

        #region Public Events

        public event EventHandler<ActivationProcessedEventArgs> ActivationProcessed = delegate { };

        public event EventHandler<DialogClosedEventArgs> Closed;

        #endregion

        #region Public Properties

        [Inject]
        public IDialogService DialogService { get; set; }

        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }
            set
            {
                if (_isOpen != value)
                {
                    _isOpen = value;
                    NotifyOfPropertyChange(() => IsOpen);
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        public void ActivateItem(object item)
        {
            if (ActiveItem == item)
            {
                return;
            }

            ActiveItem = item as IScreen;

            SetParentOnActiveItem();

            if (ActiveItem != null)
            {
                ActiveItem.Deactivated += ActiveItemDeactivated;
                ActiveItem.Activate();
                ActivationProcessed(this, new ActivationProcessedEventArgs { Item = ActiveItem, Success = true });
            }
        }

        private void SetParentOnActiveItem()
        {
            var child = ActiveItem as IChild;
            if (child != null)
            {
                child.Parent = this;
            }
        }

        public void DeactivateItem(object item, bool close)
        {
            var guard = item as IGuardClose;
            if (guard != null)
            {
                guard.CanClose(
                    result =>
                    {
                        if (result)
                        {
                            CloseActiveItemCore();
                        }
                    });
            }
            else
            {
                CloseActiveItemCore();
            }
        }

        #endregion

        #region Methods

        private async void ActiveItemDeactivated(object sender, DeactivationEventArgs e)
        {
            ((Screen)ActiveItem).Deactivated -= ActiveItemDeactivated;

            DeactivateItem(ActiveItem, true);
        }

        private void CloseActiveItemCore()
        {
            var oldItem = ActiveItem;
            OnClosed(oldItem);
        }

        private void OnClosed(object closedItem)
        {
            if (Closed != null)
            {
                Closed(this, new DialogClosedEventArgs
                {
                    ClosedItem = closedItem
                });
            }
        }

        #endregion
    }
}
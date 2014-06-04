using System;
using Caliburn.Micro;

namespace Omnipaste.Dialog
{
    public class DialogClosedEventArgs : EventArgs
    {
        public object ClosedItem { get; set; }
    }

    public class DialogViewModel : Conductor<IScreen>, IDialogViewModel
    {
        public event EventHandler<ActivationProcessedEventArgs> ActivationProcessed = delegate { };

        public event EventHandler<DialogClosedEventArgs> Closed;

        private bool _isOpen;

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

        public void ActivateItem(object item)
        {
            if (ActiveItem == item)
            {
                return;
            }

            ActiveItem = item as IScreen;

            var child = ActiveItem as IChild;
            if (child != null)
                child.Parent = this;

            if (ActiveItem != null)
                ActiveItem.Activate();

            NotifyOfPropertyChange(() => ActiveItem);
            ActivationProcessed(this, new ActivationProcessedEventArgs { Item = ActiveItem, Success = true });
        }

        public void DeactivateItem(object item, bool close)
        {
            var guard = item as IGuardClose;
            if (guard != null)
            {
                guard.CanClose(result =>
                {
                    if (result)
                        CloseActiveItemCore();
                });
            }
            else
            {
                CloseActiveItemCore();
            }
        }

        private void CloseActiveItemCore()
        {
            var oldItem = ActiveItem;
            ActivateItem(null);
            oldItem.Deactivate(true);
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
    }
}
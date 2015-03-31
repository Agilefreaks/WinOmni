namespace OmniUI.Dialog
{
    using System;
    using Caliburn.Micro;

    public class DialogViewModel : Conductor<IScreen>.Collection.OneActive, IDialogViewModel
    {
        #region Fields

        private bool _isOpen;

        #endregion

        #region Public Events

        public event EventHandler<EventArgs> Closed;

        #endregion

        #region Public Properties

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
                    NotifyOfPropertyChange();
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
            }
        }

        #endregion

        #region Methods

        private void ActiveItemDeactivated(object sender, DeactivationEventArgs e)
        {
            ((Screen)ActiveItem).Deactivated -= ActiveItemDeactivated;
            ActiveItem = null;

            if (Closed != null)
            {
                Closed(this, new EventArgs());
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

        #endregion
    }
}
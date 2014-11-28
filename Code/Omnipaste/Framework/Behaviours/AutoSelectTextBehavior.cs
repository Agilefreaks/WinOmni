namespace Omnipaste.Framework.Behaviours
{
    using System.Windows.Controls;
    using System.Windows.Interactivity;

    public class AutoSelectTextBehavior : Behavior<TextBox>
    {
        #region Methods

        protected override void OnAttached()
        {
            AssociatedObject.SelectAll();
        }

        #endregion
    }
}
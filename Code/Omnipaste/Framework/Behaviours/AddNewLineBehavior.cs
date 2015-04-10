namespace Omnipaste.Framework.Behaviours
{
    using System.Windows;
    using OmniUI.Framework.Behaviors;

    public class AddNewLineBehavior : HotKeyBehavior
    {
        #region Static Fields

        public static readonly DependencyProperty TextToAddProperty = DependencyProperty.Register(
            "TextToAdd",
            typeof(string),
            typeof(AddNewLineBehavior),
            new PropertyMetadata(default(string)));

        #endregion

        #region Public Properties

        public string TextToAdd
        {
            get
            {
                return (string)GetValue(TextToAddProperty);
            }
            set
            {
                SetValue(TextToAddProperty, value);
            }
        }

        #endregion

        #region Methods

        protected override void ExecuteAction()
        {
            var initialCaretIndex = AssociatedObject.CaretIndex;
            AssociatedObject.Text = AssociatedObject.Text.Insert(AssociatedObject.CaretIndex, TextToAdd);
            AssociatedObject.CaretIndex = initialCaretIndex + TextToAdd.Length;
        }

        #endregion
    }
}
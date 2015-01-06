namespace Omnipaste.Framework.Behaviours
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Interactivity;
    using Omnipaste.Helpers;

    public class TextProcessingBehavior : Behavior<RichTextBox>
    {
        #region Static Fields

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(TextProcessingBehavior),
            new PropertyMetadata(default(string), OnTextChanged));

        #endregion

        #region Public Properties

        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        #endregion

        #region Methods

        private static void OnTextChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((TextProcessingBehavior)dependencyObject).OnTextChanged();
        }

        private void OnTextChanged()
        {
            var textParts = new TextParser().Parse(Text);
            var paragraph = new Paragraph();
            foreach (var textPart in textParts)
            {
                switch (textPart.Item1)
                {
                    case TextPartTypeEnum.Hyperlink:
                        var hyperlink = new Hyperlink { NavigateUri = new Uri(textPart.Item2) };
                        hyperlink.Inlines.Add(textPart.Item2);
                        var behaviors = Interaction.GetBehaviors(hyperlink);
                        behaviors.Add(new AutoFollowLinkBehavior());
                        paragraph.Inlines.Add(hyperlink);
                        break;
                    default :
                        paragraph.Inlines.Add(textPart.Item2);
                        break;

                }
            }

            AssociatedObject.Document.Blocks.Add(paragraph);
        }

        #endregion
    }
}
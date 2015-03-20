namespace Omnipaste.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;

    public class TokenizingControl : RichTextBox
    {
        public TokenizingControl()
        {
            TextChanged += OnTokenTextChanged;
        }

        public IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        public string DisplayPath
        {
            get
            {
                return (string)GetValue(DisplayPathProperty);
            }
            set
            {
                SetValue(DisplayPathProperty, value);
            }
        }

        public DataTemplate TokenTemplate
        {
            get
            {
                return (DataTemplate)GetValue(TokenTemplateProperty);
            }
            set
            {
                SetValue(TokenTemplateProperty, value);
            }
        }

        public Func<string, object> TokenMatcher { get; set; }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (TokenizingControl)d;
            var newValue = (IEnumerable)e.NewValue;

            SubscribeNewHandlers(newValue as INotifyCollectionChanged, control);
            UnsubscribeOldHandlers(e);

            foreach (var item in newValue)
            {
                AddItemControl(control, item);
            }
        }

        private static void AddItemControl(TokenizingControl control, object item)
        {
            var currentParagraph = control.CaretPosition.Paragraph;
            if (currentParagraph != null)
            {
                currentParagraph.Inlines.Add(control.CreateToken(item));
            }

            control.CaretPosition = control.CaretPosition.DocumentEnd;
        }

        private static void SubscribeNewHandlers(INotifyCollectionChanged newValue, TokenizingControl control)
        {
            var paragraph = control.CaretPosition.Paragraph;

            if (notifyCollectionChangedEventHandler == null)
            {
                notifyCollectionChangedEventHandler = (s, args) =>
                    {
                        if (args.Action == NotifyCollectionChangedAction.Add)
                        {
                            AddItemControl(control, args.NewItems[0]);
                        }
                        else if (args.Action == NotifyCollectionChangedAction.Remove)
                        {
                            var itemToRemove =
                                paragraph.Inlines.FirstOrDefault(
                                    i =>
                                        {
                                            var inlineUiContainer = (InlineUIContainer)i;
                                            var contentPresenter = (ContentPresenter)inlineUiContainer.Child;
                                            
                                            return contentPresenter.Content == args.OldItems[0];
                                        });

                            if (itemToRemove != null)
                            {
                                paragraph.Inlines.Remove(itemToRemove);
                            }
                        }
                    };
            }

            if (newValue != null)
            {
                newValue.CollectionChanged += notifyCollectionChangedEventHandler;
            }
        }

        private static void UnsubscribeOldHandlers(DependencyPropertyChangedEventArgs e)
        {
            var oldValue = e.OldValue as INotifyCollectionChanged;
            if (oldValue != null)
            {
                oldValue.CollectionChanged -= notifyCollectionChangedEventHandler;
            }
        }

        private InlineUIContainer CreateToken(object item)
        {
            var presenter = new ContentPresenter { Content = item, ContentTemplate = TokenTemplate };

            // BaselineAlignment is needed to align with Run
            return new InlineUIContainer(presenter) { BaselineAlignment = BaselineAlignment.TextBottom };
        }

        private void OnTokenTextChanged(object sender, TextChangedEventArgs e)
        {
            var text = CaretPosition.GetTextInRun(LogicalDirection.Backward);
            var token = text.EndsWith(";") ? text.Substring(0, text.Length - 1).Trim().ToUpper() : null;

            if (token != null)
            {
                ReplaceTextWithToken(text, token);
            }
        }

        private void ReplaceTextWithToken(string inputText, object token)
        {
            // Remove the handler temporarily as we will be modifying tokens below, causing more TextChanged events
            TextChanged -= OnTokenTextChanged;

            var para = CaretPosition.Paragraph;

            var matchedRun = para.Inlines.FirstOrDefault(
                inline =>
                    {
                        var run = inline as Run;
                        return (run != null && run.Text.EndsWith(inputText));
                    }) as Run;
            if (matchedRun != null) // Found a Run that matched the inputText
            {
                // Remove only if the Text in the Run is the same as inputText, else split up
                if (matchedRun.Text == inputText)
                {
                    para.Inlines.Remove(matchedRun);
                }
                else // Split up
                {
                    var index = matchedRun.Text.IndexOf(inputText) + inputText.Length;
                    var tailEnd = new Run(matchedRun.Text.Substring(index));
                    para.Inlines.InsertAfter(matchedRun, tailEnd);
                    para.Inlines.Remove(matchedRun);
                }
            }
            if (AddNewToken != null)
            {
                AddNewToken(this, new TokenEventArgs(token.ToString()));
            }

            TextChanged += OnTokenTextChanged;
        }

        private InlineUIContainer CreateTokenContainer(string inputText, object token)
        {
            // Note: we are not using the inputText here, but could be used in future

            var presenter = new ContentPresenter { Content = token, ContentTemplate = TokenTemplate };

            // BaselineAlignment is needed to align with Run
            return new InlineUIContainer(presenter) { BaselineAlignment = BaselineAlignment.TextBottom };
        }

        public event OnAddNewTokenEventHandler AddNewToken;

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource",
            typeof(INotifyCollectionChanged),
            typeof(TokenizingControl),
            new FrameworkPropertyMetadata(OnItemsSourceChanged));

        public static readonly DependencyProperty DisplayPathProperty = DependencyProperty.Register(
            "DisplayPath",
            typeof(string),
            typeof(TokenizingControl));

        public static readonly DependencyProperty TokenTemplateProperty = DependencyProperty.Register(
            "TokenTemplate",
            typeof(DataTemplate),
            typeof(TokenizingControl));

        private static NotifyCollectionChangedEventHandler notifyCollectionChangedEventHandler;
    }

    public delegate void OnRemoveTokenEventHandler(object sender, TokenEventArgs args);

    public delegate void OnAddNewTokenEventHandler(object sender, TokenEventArgs args);

    public class TokenEventArgs : EventArgs
    {
        public TokenEventArgs(string tokenIdentifier)
        {
            TokenIdentifier = tokenIdentifier;
        }

        public string TokenIdentifier { get; set; }
    }
}
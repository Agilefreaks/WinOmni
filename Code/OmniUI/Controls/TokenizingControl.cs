namespace OmniUI.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;

    public class TokenizingControl : RichTextBox
    {
        private const string DefaultTokenSeparator = " ";

        public static readonly DependencyProperty TokenizerProperty = DependencyProperty.Register(
            "Tokenizer",
            typeof(Func<string, ITokenizeResult>),
            typeof(TokenizingControl),
            new PropertyMetadata(default(Func<string, ITokenizeResult>)));

        public static readonly DependencyProperty TokenTemplateProperty = DependencyProperty.Register(
            "TokenTemplate",
            typeof(DataTemplate),
            typeof(TokenizingControl),
            new PropertyMetadata(default(DataTemplate)));

        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
            "SelectedItems",
            typeof(IEnumerable),
            typeof(TokenizingControl),
            new PropertyMetadata(default(IEnumerable), SelectedItemsChanged));

        private bool _isUpdatingTokens;

        private bool _isUpdatingText;

        public TokenizingControl()
        {
            TextChanged += OnTokenTextChanged;
            Tokenizer = DefaultTokenMathcer;
            SelectedItems = new List<object>();
            IsUndoEnabled = false;
            IsDocumentEnabled = true;
        }

        public Func<string, ITokenizeResult> Tokenizer
        {
            get
            {
                return (Func<string, ITokenizeResult>)GetValue(TokenizerProperty);
            }
            set
            {
                SetValue(TokenizerProperty, value);
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

        public IEnumerable SelectedItems
        {
            get
            {
                return (IEnumerable)GetValue(SelectedItemsProperty);
            }
            set
            {
                SetValue(SelectedItemsProperty, value);
            }
        }

        private static ITokenizeResult DefaultTokenMathcer(string text)
        {
            var tokenizeResult = new TokenizeResult();
            if (text.EndsWith(DefaultTokenSeparator))
            {
                tokenizeResult.Tokens = new[] { text.Substring(0, text.Length - 1) };
                tokenizeResult.NonTokenizedText = string.Empty;
            }

            return tokenizeResult;
        }

        private static void SelectedItemsChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs eventArgs)
        {
            var tokenizingControl = dependencyObject as TokenizingControl;
            if (tokenizingControl == null)
            {
                return;
            }

            var oldCollectionWithNotifications = eventArgs.OldValue as INotifyCollectionChanged;
            if (oldCollectionWithNotifications != null)
            {
                tokenizingControl.UnhookSelectedItems(oldCollectionWithNotifications);
            }

            var newCollectionWithNotifications = eventArgs.NewValue as INotifyCollectionChanged;
            if (newCollectionWithNotifications != null)
            {
                tokenizingControl.HookSelectedItems(newCollectionWithNotifications);
            }

            tokenizingControl.InitializeFromSelectedItems();
        }

        private static IEnumerable<InlineUIContainer> GetContainersForItems(Paragraph paragraph, IEnumerable items)
        {
            var itemsList = items.Cast<object>().ToList();
            return paragraph.Inlines.OfType<InlineUIContainer>()
                .Where(
                    container =>
                    container.Child is ContentPresenter
                    && itemsList.Contains(((ContentPresenter)container.Child).Content))
                .ToList();
        }

        private void HookSelectedItems(INotifyCollectionChanged collectionWithNotifications)
        {
            collectionWithNotifications.CollectionChanged += OnSelectedItemsChanged;
        }

        private void UnhookSelectedItems(INotifyCollectionChanged collectionWithNotifications)
        {
            collectionWithNotifications.CollectionChanged -= OnSelectedItemsChanged;
        }

        private void InitializeFromSelectedItems()
        {
            ClearContent();
            var paragraph = CreateDefaultParagraph();
            if (SelectedItems != null)
            {
                AddTokenElements(SelectedItems.Cast<object>().ToList(), paragraph);
            }
        }

        private void OnSelectedItemsChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            if (_isUpdatingTokens)
            {
                return;
            }

            var paragraph = Document.Blocks.OfType<Paragraph>().FirstOrDefault() ?? CreateDefaultParagraph();
            RemoveTokenElements(eventArgs.OldItems, paragraph);
            AddTokenElements(eventArgs.NewItems, paragraph);
        }

        private void OnTokenTextChanged(object sender, TextChangedEventArgs eventArgs)
        {
            if (_isUpdatingText)
            {
                return;
            }

            DeselectOldItems();
            UpdateTokens();
        }

        private void DeselectOldItems()
        {
            var selectedItemsList = SelectedItems as IList;
            var paragraph = Document.Blocks.OfType<Paragraph>().FirstOrDefault();
            if (selectedItemsList == null || paragraph == null)
            {
                return;
            }

            var existingTokens =
                paragraph.Inlines.OfType<InlineUIContainer>()
                    .Where(container => container.Child is ContentPresenter)
                    .Select(container => container.Child)
                    .OfType<ContentPresenter>()
                    .Select(contentPresenter => contentPresenter.Content)
                    .ToList();
            var deletedTokens = selectedItemsList.Cast<object>().Where(item => !existingTokens.Contains(item)).ToList();
            foreach (var item in deletedTokens)
            {
                selectedItemsList.Remove(item);
            }
        }

        private void UpdateTokens()
        {
            _isUpdatingTokens = true;
            if (Tokenizer != null)
            {
                var text = CaretPosition.GetTextInRun(LogicalDirection.Backward);
                var tokenizeResult = Tokenizer(text);
                ReplaceTextWithTokens(text, tokenizeResult);
                var tokensList = SelectedItems as IList;
                if (tokensList != null)
                {
                    foreach (var token in tokenizeResult.Tokens)
                    {
                        tokensList.Add(token);
                    }
                }
            }

            _isUpdatingTokens = false;
        }

        private Paragraph CreateDefaultParagraph()
        {
            var paragraph = new Paragraph();
            Document.Blocks.Add(paragraph);

            return paragraph;
        }

        private void ReplaceTextWithTokens(string inputText, ITokenizeResult tokenizeResult)
        {
            _isUpdatingText = true;
            var paragraph = CaretPosition.Paragraph ?? CreateDefaultParagraph();
            var matchedRun = paragraph.Inlines.OfType<Run>().FirstOrDefault(run => run.Text.EndsWith(inputText));
            if (matchedRun != null)
            {
                foreach (var tokenContainer in tokenizeResult.Tokens.Select(CreateTokenContainer))
                {
                    paragraph.Inlines.InsertBefore(matchedRun, tokenContainer);
                }

                if (tokenizeResult.NonTokenizedText != string.Empty)
                {
                    var tailEnd = new Run(tokenizeResult.NonTokenizedText);
                    paragraph.Inlines.InsertBefore(matchedRun, tailEnd);
                }

                paragraph.Inlines.Remove(matchedRun);
            }

            _isUpdatingText = false;
        }

        private void ClearContent()
        {
            Document.Blocks.Clear();
        }

        private void RemoveTokenElements(IList items, Paragraph paragraph)
        {
            items = items ?? new List<object>();
            var containersToRemove = GetContainersForItems(paragraph, items);
            foreach (var container in containersToRemove)
            {
                paragraph.Inlines.Remove(container);
            }
        }

        private void AddTokenElements(IList items, Paragraph paragraph)
        {
            var itemLists = items != null ? items.Cast<object>() : new List<object>();
            foreach (var tokenContainer in itemLists.Select(CreateTokenContainer))
            {
                paragraph.Inlines.Add(tokenContainer);
            }
        }

        private InlineUIContainer CreateTokenContainer(object token)
        {
            var presenter = new ContentPresenter { Content = token, ContentTemplate = TokenTemplate };

            return new InlineUIContainer(presenter) { BaselineAlignment = BaselineAlignment.TextBottom };
        }
    }
}
namespace OmniHolidays.MessagesWorkspace.Behaviors
{
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Interactivity;
    using Humanizer;
    using OmniUI.Helpers;

    public class FormatPlaceholdersBehavior : Behavior<TextBlock>
    {
        protected override void OnAttached()
        {
            var regex = new Regex("(%\\w*%)");
            var items = regex.Split(AssociatedObject.Text);
            AssociatedObject.Text = string.Empty;

            foreach (var part in items)
            {
                Run run;
                if (regex.IsMatch(part))
                {
                    var friendlyPlaceholder = part.Replace("%", string.Empty).Humanize();
                    run = new Run(friendlyPlaceholder)
                              {
                                  Style =
                                      ResourceHelper.GetByKey<Style>(
                                          "TemplatePlaceholderTextBlockStyle")
                              };
                }
                else
                {
                    run = new Run(part);
                }

                AssociatedObject.Inlines.Add(run);
            }
        }
    }
}
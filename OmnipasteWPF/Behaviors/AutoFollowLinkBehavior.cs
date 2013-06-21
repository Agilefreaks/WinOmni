﻿namespace OmnipasteWPF.Behaviors
{
    using System.Diagnostics;
    using System.Windows.Documents;
    using System.Windows.Interactivity;
    using System.Windows.Navigation;

    public class AutoFollowLinkBehavior : Behavior<Hyperlink>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.RequestNavigate += OnRequestNavigate;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.RequestNavigate -= OnRequestNavigate;
        }

        private void OnRequestNavigate(object sender, RequestNavigateEventArgs requestNavigateEventArgs)
        {
            Process.Start(new ProcessStartInfo(requestNavigateEventArgs.Uri.AbsoluteUri));
        }
    }
}
namespace Omnipaste.Framework.Behaviours
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Interactivity;

    public class FirstItemSelectedBehavior : Behavior<Selector>
    {
        protected override void OnAttached()
        {
            var itemSourcePropertyDescriptor = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(Selector));

            itemSourcePropertyDescriptor.AddValueChanged(AssociatedObject, SelectFirstItem);
        }

        private void SelectFirstItem(object sender, EventArgs e)
        {
            if (AssociatedObject.Items != null)
            {
                var item = AssociatedObject.Items.Cast<object>().FirstOrDefault();
                AssociatedObject.SelectedItem = item;
            }
        }
    }
}

namespace Omnipaste.ClippingList
{
    using System.Collections.Specialized;
    using Caliburn.Micro;

    public class LimitableBindableCollection<T> : BindableCollection<T>
    {
        public LimitableBindableCollection(int limit)
        {
            Limit = limit;
        }

        public int Limit { get; private set; }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            if (e.Action != NotifyCollectionChangedAction.Add)
            {
                return;
            }

            while (Count > Limit)
            {
                RemoveAt(Count - 1);
            }
        }
    }
}
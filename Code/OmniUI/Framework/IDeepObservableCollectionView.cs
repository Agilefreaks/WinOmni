namespace OmniUI.Framework
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;

    public interface IDeepObservableCollectionView : INotifyCollectionChanged, IEnumerable
    {
        Predicate<object> Filter { get; set; }
    }
}
namespace OmniUI.Framework
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Windows.Data;
    using Caliburn.Micro;

    public class DeepObservableCollectionView<T> : IDeepObservableCollectionView
        where T : INotifyPropertyChanged
    {
        #region Fields

        private readonly CollectionViewSource _collectionViewSource;

        #endregion

        #region Constructors and Destructors

        public DeepObservableCollectionView(IObservableCollection<T> source)
        {
            _collectionViewSource = new CollectionViewSource { Source = source };
            foreach (var item in source)
            {
                item.PropertyChanged += ItemPropertyChanged;
            }
            source.CollectionChanged += OnCollectionChanged;
        }

        #endregion

        #region Public Events

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region Public Properties

        public Predicate<object> Filter
        {
            get
            {
                return _collectionViewSource.View.Filter;
            }
            set
            {
                _collectionViewSource.View.Filter = value;
            }
        }

        public int Count
        {
            get
            {
                return ((ListCollectionView)_collectionViewSource.View).Count;
            }
        }

        #endregion

        #region Public Methods and Operators

        public IEnumerator GetEnumerator()
        {
            return _collectionViewSource.View.GetEnumerator();
        }

        #endregion

        #region Methods

        protected void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            if (eventArgs.NewItems != null)
            {
                foreach (INotifyPropertyChanged item in eventArgs.NewItems)
                {
                    item.PropertyChanged += ItemPropertyChanged;
                }
            }

            if (eventArgs.OldItems != null)
            {
                foreach (INotifyPropertyChanged item in eventArgs.OldItems)
                {
                    item.PropertyChanged -= ItemPropertyChanged;
                }
            }

            if (CollectionChanged != null)
            {
                CollectionChanged(this, eventArgs);
            }
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSelected")
            {
                _collectionViewSource.View.Refresh();
                var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                OnCollectionChanged(this, eventArgs);
            }
        }

        #endregion
    }
}
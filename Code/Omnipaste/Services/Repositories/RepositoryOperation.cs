namespace Omnipaste.Services.Repositories
{
    using System;

    public class RepositoryOperation
    {
        public RepositoryMethodEnum RepositoryMethod { get; set; }

        private object Item { get; set; }

        public RepositoryOperation(RepositoryMethodEnum repositoryMethod)
        {
            RepositoryMethod = repositoryMethod;
        }

        protected RepositoryOperation()
        {
        }

        public T GetItem<T>()
        {
            return (T) Item;
        }

        public void SetItem<T>(T item)
        {
            Item = item;
        }
    }

    public class RepositoryOperation<T> : RepositoryOperation
    {
        public RepositoryOperation(RepositoryMethodEnum repositoryMethod, T item)
            : base (repositoryMethod)
        {
            RepositoryMethod = repositoryMethod;
            SetItem(item);
        }

        private RepositoryOperation()
        {
        }

        public T Item
        {
            get
            {
                return GetItem<T>();
            }

            set
            {
                SetItem(value);
            }
        }

        public static RepositoryOperation<T> Empty()
        {
            return new RepositoryOperation<T>();
        }
    }
}
namespace Omnipaste.Services.Repositories
{
    public class RepositoryOperation<T>
    {
        public RepositoryOperation(RepositoryMethodEnum repositoryMethod, T item)
        {
            RepositoryMethod = repositoryMethod;
            Item = item;
        }

        private RepositoryOperation()
        {
        }

        public RepositoryMethodEnum RepositoryMethod { get; set; }

        public T Item { get; set; }

        public static RepositoryOperation<T> Empty()
        {
            return new RepositoryOperation<T>();
        }
    }
}
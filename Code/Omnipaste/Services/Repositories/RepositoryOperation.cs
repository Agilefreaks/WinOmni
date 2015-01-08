namespace Omnipaste.Services.Repositories
{
    public class RepositoryOperation<T>
    {
        public RepositoryMethodEnum RepositoryMethod { get; set; }

        public T Item { get; set; }

        public RepositoryOperation(RepositoryMethodEnum repositoryMethod, T item)
        {
            RepositoryMethod = repositoryMethod;
            Item = item;
        }
    }
}
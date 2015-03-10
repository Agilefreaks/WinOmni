namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using Omnipaste.Models;

    public class SercureRepository<T> : IRepository<T>
        where T : BaseModel
    {
        public IObservable<RepositoryOperation<T>> OperationObservable
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IObservable<RepositoryOperation<T>> Save(T item)
        {
            throw new NotImplementedException();
        }

        public IObservable<RepositoryOperation<T>> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public IObservable<T> Get(string id)
        {
            throw new NotImplementedException();
        }

        public IObservable<T> Get(Func<T, bool> match)
        {
            throw new NotImplementedException();
        }

        public IObservable<IEnumerable<T>> GetAll()
        {
            throw new NotImplementedException();
        }

        public IObservable<IEnumerable<T>> GetAll(Func<T, bool> filter)
        {
            throw new NotImplementedException();
        }
    }
}
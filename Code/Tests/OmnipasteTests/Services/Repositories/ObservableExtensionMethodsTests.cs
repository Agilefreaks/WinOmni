﻿namespace OmnipasteTests.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Subjects;
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Services.Repositories;

    [TestFixture]
    public class ObservableExtensionMethodsTests
    {
        private ReplaySubject<RepositoryOperation<string>> _repositoryOperationSubject;

        [SetUp]
        public void SetUp()
        {
            _repositoryOperationSubject = new ReplaySubject<RepositoryOperation<string>>();
        }

        [Test]
        public void OnSaved_Always_ReturnsOnlySaveOperations()
        {
            _repositoryOperationSubject.OnNext(new RepositoryOperation<string>(RepositoryMethodEnum.Save, "1"));
            _repositoryOperationSubject.OnNext(new RepositoryOperation<string>(RepositoryMethodEnum.Delete, "2"));
            _repositoryOperationSubject.OnNext(new RepositoryOperation<string>(RepositoryMethodEnum.Save, "3"));

            var result = new List<RepositoryOperation<string>>(); 
            _repositoryOperationSubject.Saved().Subscribe(e => result.Add(e));

            result.Count.Should().Be(2);
            result.All(m => m.RepositoryMethod == RepositoryMethodEnum.Save).Should().BeTrue();
        }

        [Test]
        public void Deleted_Always_ReturnsOnlySaveOperations()
        {
            _repositoryOperationSubject.OnNext(new RepositoryOperation<string>(RepositoryMethodEnum.Delete, "1"));
            _repositoryOperationSubject.OnNext(new RepositoryOperation<string>(RepositoryMethodEnum.Save, "2"));
            _repositoryOperationSubject.OnNext(new RepositoryOperation<string>(RepositoryMethodEnum.Delete, "3"));

            var result = new List<RepositoryOperation<string>>(); 
            _repositoryOperationSubject.Deleted().Subscribe(e => result.Add(e));

            result.Count.Should().Be(2);
            result.All(m => m.RepositoryMethod == RepositoryMethodEnum.Delete).Should().BeTrue();
        }
    }
}

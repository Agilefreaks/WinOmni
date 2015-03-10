﻿namespace OmnipasteTests.Services.Repositories
{
    using NUnit.Framework;
    using Omnipaste.Services.Repositories;
    using OmnipasteTests.Helpers;

    [TestFixture]
    public class InMemoryRepositoryTests : BaseRepositoryTest
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _subject = new TestModelRepository();
        }

        #region Nested type: TestModelRepository

        public class TestModelRepository : InMemoryRepository<TestModel>
        {
        }

        #endregion
    }
}
namespace OmnipasteTests.Services.Repositories
{
    using NUnit.Framework;
    using Omnipaste.Framework.Services.Repositories;
    using OmnipasteTests.Framework.Helpers;

    [TestFixture]
    public class InMemoryRepositoryTests : BaseRepositoryTests
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            Subject = new TestModelRepository();
        }

        #region Nested type: TestModelRepository

        public class TestModelRepository : InMemoryRepository<TestModel>
        {
        }

        #endregion
    }
}
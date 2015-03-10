namespace OmnipasteTests.Services.Repositories
{
    using NUnit.Framework;
    using Omnipaste.Services.Repositories;
    using OmnipasteTests.Helpers;

    [TestFixture]
    public class SecurePermanentRepositoryTest : BaseRepositoryTest
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _subject = new TestModelRepository();
        }

        public class TestModelRepository : SecurePermanentRepository<TestModel>
        {
            public TestModelRepository()
                : base("test")
            {
            }
        }
    }
}
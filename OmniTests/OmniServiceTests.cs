namespace OmniTests
{
    using NUnit.Framework;
    using Ninject.MockingKernel.Moq;
    using Omni;
    using OmniCommon.Services;
    using OmniSync;

    [TestFixture]
    public class OmniServiceTests
    {
        private OmniService _subject;

        [SetUp]
        public void SetUp()
        {
            var moqMockingKernel = new MoqMockingKernel();
            var mockOmniSyncService = moqMockingKernel.GetMock<IOmniSyncService>();


        }

        [Test]
        public void Start_CallsOmniSyncServiceStart()
        {
            
        }
    }
}
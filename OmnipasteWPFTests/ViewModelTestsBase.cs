namespace OmnipasteWPFTests
{
    using Cinch;
    using Moq;

    public class ViewModelTestsBase
    {
        protected Mock<IIOCProvider> MockIOCProvider { get; set; }

        protected Mock<ILogger> MockIOCLogger { get; set; }

        protected Mock<IUIVisualizerService> MockUiVisualizerService { get; set; }

        protected Mock<IMessageBoxService> MockMessageBoxService { get; set; }

        protected Mock<IOpenFileService> MockOpenFileService { get; set; }

        protected Mock<ISaveFileService> MockSaveFileService { get; set; }

        public virtual void Setup()
        {
            MockIOCProvider = new Mock<IIOCProvider>();
            MockIOCLogger = new Mock<ILogger>();
            MockUiVisualizerService = new Mock<IUIVisualizerService>();
            MockMessageBoxService = new Mock<IMessageBoxService>();
            MockOpenFileService = new Mock<IOpenFileService>();
            MockSaveFileService = new Mock<ISaveFileService>();

            MockIOCProvider.Setup(x => x.GetTypeFromContainer<ILogger>()).Returns(MockIOCLogger.Object);
            MockIOCProvider.Setup(x => x.GetTypeFromContainer<IMessageBoxService>()).Returns(MockMessageBoxService.Object);
            MockIOCProvider.Setup(x => x.GetTypeFromContainer<IOpenFileService>()).Returns(MockOpenFileService.Object);
            MockIOCProvider.Setup(x => x.GetTypeFromContainer<ISaveFileService>()).Returns(MockSaveFileService.Object);
            MockIOCProvider.Setup(x => x.GetTypeFromContainer<IUIVisualizerService>()).Returns(MockUiVisualizerService.Object);
        }
    }
}
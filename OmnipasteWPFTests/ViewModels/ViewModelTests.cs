namespace OmnipasteWPFTests.ViewModels
{
    using Cinch;
    using Moq;

    public class ViewModelTests
    {
        protected Mock<IIOCProvider> MockIOCProvider { get; set; }

        protected Mock<ILogger> MockIOCLogger { get; set; }

        protected Mock<IUIVisualizerService> MockUiVisualizerService { get; set; }

        protected Mock<IMessageBoxService> MockMessageBoxService { get; set; }

        protected Mock<IOpenFileService> MockOpenFileService { get; set; }

        protected Mock<ISaveFileService> MockSaveFileService { get; set; }

        public virtual void Setup()
        {
            this.MockIOCProvider = new Mock<IIOCProvider>();
            this.MockIOCLogger = new Mock<ILogger>();
            this.MockUiVisualizerService = new Mock<IUIVisualizerService>();
            this.MockMessageBoxService = new Mock<IMessageBoxService>();
            this.MockOpenFileService = new Mock<IOpenFileService>();
            this.MockSaveFileService = new Mock<ISaveFileService>();

            this.MockIOCProvider.Setup(x => x.GetTypeFromContainer<ILogger>()).Returns(this.MockIOCLogger.Object);
            this.MockIOCProvider.Setup(x => x.GetTypeFromContainer<IMessageBoxService>()).Returns(this.MockMessageBoxService.Object);
            this.MockIOCProvider.Setup(x => x.GetTypeFromContainer<IOpenFileService>()).Returns(this.MockOpenFileService.Object);
            this.MockIOCProvider.Setup(x => x.GetTypeFromContainer<ISaveFileService>()).Returns(this.MockSaveFileService.Object);
            this.MockIOCProvider.Setup(x => x.GetTypeFromContainer<IUIVisualizerService>()).Returns(this.MockUiVisualizerService.Object);
        }
    }
}
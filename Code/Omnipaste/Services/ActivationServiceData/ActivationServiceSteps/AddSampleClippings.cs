namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using Clipboard.Handlers.WindowsClipboard;

    public class AddSampleClippings : SynchronousStepBase
    {
        private readonly IWindowsClipboardWrapper _windowsClipboardWrapper;

        public AddSampleClippings(IWindowsClipboardWrapper windowsClipboardWrapper)
        {
            _windowsClipboardWrapper = windowsClipboardWrapper;
        }

        protected override IExecuteResult ExecuteSynchronously()
        {
            _windowsClipboardWrapper.SetData(Properties.Resources.SampleLocalClipping);

            return new ExecuteResult(SimpleStepStateEnum.Successful);
        }
    }
}
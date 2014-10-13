namespace InstallerCustomActions.ClickOnceMigration
{
    using CustomizedClickOnce.Common;

    public class CloseRunningInstanceTask : IMigrationTask
    {
        private readonly IClickOnceHelper _clickOnceHelper;

        public CloseRunningInstanceTask(IClickOnceHelper clickOnceHelper)
        {
            _clickOnceHelper = clickOnceHelper;
        }

        public MigrationStepResultEnum Execute()
        {
            try
            {
                _clickOnceHelper.KillActiveProcesses();
            }
            catch
            {
            }

            return MigrationStepResultEnum.Success;
        }
    }
}
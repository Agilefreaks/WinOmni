namespace InstallerCustomActions.ClickOnceMigration
{
    using CustomizedClickOnce.Common;

    public class RemoveShortcutFromStartupTask : IMigrationTask
    {
        private readonly IClickOnceHelper _clickOnceHelper;

        public RemoveShortcutFromStartupTask(IClickOnceHelper clickOnceHelper)
        {
            _clickOnceHelper = clickOnceHelper;
        }

        public MigrationStepResultEnum Execute()
        {
            try
            {
                _clickOnceHelper.RemoveShortcutFromStartup();
            }
            catch
            {
            }

            return MigrationStepResultEnum.Success;
        }
 
    }
}
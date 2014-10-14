namespace InstallerCustomActions.ClickOnceMigration
{
    using CustomizedClickOnce.Common;

    public class RestoreOriginalUninstallerTask : IMigrationTask
    {
        private readonly IClickOnceHelper _clickOnceHelper;

        public RestoreOriginalUninstallerTask(IClickOnceHelper clickOnceHelper)
        {
            _clickOnceHelper = clickOnceHelper;
        }

        public MigrationStepResultEnum Execute()
        {
            var result = MigrationStepResultEnum.RestoreOriginalUninstallerError;

            try
            {
                if (_clickOnceHelper.RestoreOriginalUninstaller())
                {
                    result = MigrationStepResultEnum.Success;
                }
            }
            catch
            {
            }

            return result;
        }
 
    }
}
namespace InstallerCustomActions.ClickOnceMigration
{
    using CustomizedClickOnce.Common;

    public class RestoreOriginalUninstallerTask : MigrationTaskBase
    {
        private readonly IClickOnceHelper _clickOnceHelper;

        public RestoreOriginalUninstallerTask(IClickOnceHelper clickOnceHelper)
            : base(MigrationStepResultEnum.RestoreOriginalUninstallerError)
        {
            _clickOnceHelper = clickOnceHelper;
        }

        protected override MigrationStepResultEnum ExecuteCore()
        {
            return _clickOnceHelper.RestoreOriginalUninstaller() ? MigrationStepResultEnum.Success : StateOnFail;
        }
    }
}
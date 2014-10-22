namespace InstallerCustomActions.ClickOnceMigration
{
    using CustomizedClickOnce.Common;

    public class RemoveShortcutFromStartupTask : MigrationTaskBase
    {
        private readonly IClickOnceHelper _clickOnceHelper;

        public RemoveShortcutFromStartupTask(IClickOnceHelper clickOnceHelper)
            : base(MigrationStepResultEnum.Success)
        {
            _clickOnceHelper = clickOnceHelper;
        }

        protected override MigrationStepResultEnum ExecuteCore()
        {
            _clickOnceHelper.RemoveShortcutFromStartup();

            return MigrationStepResultEnum.Success;
        }
    }
}
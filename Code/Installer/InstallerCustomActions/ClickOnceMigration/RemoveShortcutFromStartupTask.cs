namespace InstallerCustomActions.ClickOnceMigration
{
    using InstallerCustomActions.ClickOnceHelper;

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
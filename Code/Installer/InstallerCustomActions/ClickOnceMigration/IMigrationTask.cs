namespace InstallerCustomActions.ClickOnceMigration
{
    public interface IMigrationTask
    {
        MigrationStepResultEnum Execute();
    }
}
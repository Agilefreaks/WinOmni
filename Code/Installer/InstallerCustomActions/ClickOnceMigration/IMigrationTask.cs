namespace InstallerCustomActions.ClickOnceMigration
{
    using System;

    public interface IMigrationTask
    {
        MigrationStepResultEnum Execute();

        Exception Exception { get; }
    }
}
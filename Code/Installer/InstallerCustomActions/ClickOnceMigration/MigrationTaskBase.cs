namespace InstallerCustomActions.ClickOnceMigration
{
    using System;

    public abstract class MigrationTaskBase : IMigrationTask
    {
        protected readonly MigrationStepResultEnum StateOnFail;

        public Exception Exception { get; private set; }

        protected MigrationTaskBase(MigrationStepResultEnum stateOnFail)
        {
            StateOnFail = stateOnFail;
        }

        public MigrationStepResultEnum Execute()
        {
            MigrationStepResultEnum result;

            try
            {
                result = ExecuteCore();
            }
            catch (Exception exception)
            {
                Exception = exception;
                result = StateOnFail;
            }

            return result;
        }

        protected abstract MigrationStepResultEnum ExecuteCore();
    }
}
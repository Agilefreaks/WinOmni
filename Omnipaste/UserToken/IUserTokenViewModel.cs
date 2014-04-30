namespace Omnipaste.UserToken
{
    using Omnipaste.Framework;

    public interface IUserTokenViewModel : IWorkspace
    {
        string ActivationCode { get; set; }

        void Ok();

        void Cancel();
    }
}
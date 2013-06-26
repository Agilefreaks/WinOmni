namespace Omnipaste.UserToken
{
    using Omnipaste.Framework;

    public interface IUserTokenViewModel : IWorkspace
    {
        string Token { get; set; }

        void Ok();

        void Cancel();
    }
}
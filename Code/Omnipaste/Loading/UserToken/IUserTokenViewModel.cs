namespace Omnipaste.Loading.UserToken
{
    using Caliburn.Micro;

    public interface IUserTokenViewModel : IScreen
    {
        string ActivationCode { get; set; }

        string Message { get; set; }

        void Authenticate();

        void Exit();
    }
}
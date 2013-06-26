namespace Omnipaste.UserToken
{
    using Caliburn.Micro;

    public interface IUserTokenViewModel : IScreen
    {
        string Token { get; set; }

        void Ok();
    }
}
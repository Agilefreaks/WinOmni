using Caliburn.Micro;

namespace Omnipaste.UserToken
{
    public interface IUserTokenViewModel : IScreen
    {
        string ActivationCode { get; set; }

        string Message { get; set; }

        void Register();
    }
}
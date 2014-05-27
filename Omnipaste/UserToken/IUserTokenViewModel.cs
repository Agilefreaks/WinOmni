using Omnipaste.Framework;

namespace Omnipaste.UserToken
{
    public interface IUserTokenViewModel : IFlyoutViewModel
    {
        string ActivationCode { get; set; }

        string Message { get; set; }

        void Register();
    }
}
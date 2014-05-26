using Omnipaste.Framework;

namespace Omnipaste.UserToken
{
    public interface IUserTokenViewModel : IFlyoutViewModel
    {
        string ActivationCode { get; set; }

        void Ok();

        void Cancel();
    }
}
using OmniCommon.Framework;

namespace Omnipaste.UserToken
{
    public interface IUserTokenViewModel : IWorkspace
    {
        string ActivationCode { get; set; }

        void Ok();

        void Cancel();
    }
}
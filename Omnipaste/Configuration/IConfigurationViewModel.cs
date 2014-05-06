using System.Threading.Tasks;
using OmniCommon.Framework;

namespace Omnipaste.Configuration
{
    public interface IConfigurationViewModel : IWorkspace
    {
        Task Start();
    }
}
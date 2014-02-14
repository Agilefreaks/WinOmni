using System.Threading.Tasks;

namespace Omnipaste.Configuration
{
    using Omnipaste.Framework;

    public interface IConfigurationViewModel : IWorkspace
    {
        Task Start();
    }
}
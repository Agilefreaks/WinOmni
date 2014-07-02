namespace Omnipaste.Shell.Connection
{
    using System.Threading.Tasks;
    using Caliburn.Micro;

    public interface IConnectionViewModel : IScreen
    {
        void Connect();
    }
}
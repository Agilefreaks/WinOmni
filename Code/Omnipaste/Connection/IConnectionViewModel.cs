namespace Omnipaste.Connection
{
    using System.Threading.Tasks;
    using Caliburn.Micro;

    public interface IConnectionViewModel : IScreen
    {
        Task Connect();
    }
}
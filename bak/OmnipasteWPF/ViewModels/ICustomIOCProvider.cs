namespace OmnipasteWPF.ViewModels
{
    using Cinch;

    public interface ICustomIOCProvider : IIOCProvider
    {
        void SetupLocalContainer();
    }
}
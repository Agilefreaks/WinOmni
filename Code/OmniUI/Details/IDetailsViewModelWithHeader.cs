namespace OmniUI.Details
{
    using Caliburn.Micro;

    public interface IDetailsViewModelWithHeader<out THeader, out TContent> : IScreen
        where THeader : IScreen
        where TContent : IScreen
    {
        TContent ContentViewModel { get; }

        THeader HeaderViewModel { get; }
    }
}
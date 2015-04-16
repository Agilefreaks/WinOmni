namespace OmniUI.Details
{
    public interface IDetailsViewModelWithHeader : IDetailsViewModelWithHeader<IDetailsViewModel, IDetailsViewModel>
    {
    }

    public interface IDetailsViewModelWithHeader<out THeader, out TContent> : IDetailsViewModel
        where THeader : IDetailsViewModel
        where TContent : IDetailsViewModel
    {
        TContent ContentViewModel { get; }

        THeader HeaderViewModel { get; }
    }
}
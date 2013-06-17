namespace Omnipaste
{
    public interface IConfigureDialog
    {
        void ShowDialog();

        bool Succeeded { get; }
    }
}
namespace Omnipaste.Presenters
{
    using Omnipaste.Services;

    public class UpdateInfoPresenter : Presenter<UpdateInfo>
    {
        public UpdateInfoPresenter(UpdateInfo backingModel)
            : base(backingModel)
        {
        }

        public bool WasInstalled
        {
            get
            {
                return BackingModel.WasInstalled;
            }
        }
    }
}
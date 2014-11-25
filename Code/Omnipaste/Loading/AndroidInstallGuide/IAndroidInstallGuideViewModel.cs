namespace Omnipaste.Loading.AndroidInstallGuide
{
    using System;
    using Caliburn.Micro;

    public interface IAndroidInstallGuideViewModel : IScreen
    {
        Uri AndroidInstallLink { get; set; }
    }
}
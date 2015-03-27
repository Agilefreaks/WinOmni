﻿namespace Omnipaste.WorkspaceDetails.GroupMessage
{
    using System.Collections.ObjectModel;
    using Caliburn.Micro;
    using Omnipaste.Presenters;
    using Omnipaste.WorkspaceDetails;

    public interface IGroupMessageHeaderViewModel : IWorkspaceDetailsHeaderViewModel
    {
        ObservableCollection<ContactInfoPresenter> Recipients { get; set; } 
    }
}
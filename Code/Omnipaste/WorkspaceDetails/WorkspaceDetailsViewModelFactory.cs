namespace Omnipaste.WorkspaceDetails
{
    using System;
    using Ninject;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.WorkspaceDetails.Clipping;
    using Omnipaste.WorkspaceDetails.Conversation;
    using Omnipaste.WorkspaceDetails.Version;

    public class WorkspaceDetailsViewModelFactory : IWorkspaceDetailsViewModelFactory
    {
        [Inject]
        public IKernel Kernel { get; set; }

        public IWorkspaceDetailsViewModel Create(ActivityPresenter activity)
        {
            IWorkspaceDetailsViewModel result;
            switch (activity.Type)
            {
                case ActivityTypeEnum.Clipping:
                    result = Kernel.Get<IClippingDetailsViewModel>();
                    result.Model = activity;
                    break;
                case ActivityTypeEnum.Message:
                case ActivityTypeEnum.Call:
                    result = Kernel.Get<IConversationViewModel>();
                    result.Model = activity;
                    break;
                case ActivityTypeEnum.Version:
                    result = Kernel.Get<IVersionDetailsViewModel>();
                    result.Model = activity;
                    break;
                default:
                    throw new Exception("Unknown type");
            }
            
            return result;
        }
    }
}
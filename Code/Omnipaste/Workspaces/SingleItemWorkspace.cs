﻿namespace Omnipaste.Workspaces
{
    using Caliburn.Micro;

    public abstract class SingleItemWorkspace : Conductor<IScreen>, IWorkspace
    {
        private readonly IScreen _defaultScreen;

        protected SingleItemWorkspace(IScreen defaultScreen)
        {
            _defaultScreen = defaultScreen;
        }

        public virtual string Icon { get; protected set; }

        protected override void OnActivate()
        {
            base.OnActivate();
            ActivateItem(_defaultScreen);
        }
    }
}
﻿namespace Omnipaste.Workspaces
{
    using Caliburn.Micro;

    public class Workspace : Conductor<IScreen>.Collection.AllActive, IWorkspace
    {
        private readonly IScreen _defaultScreen;

        protected Workspace(IScreen defaultScreen)
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
namespace OmniUI
{
    using Caliburn.Micro;
    using OmniCommon;

    public class OmniUiModule : ModuleBase
    {
        public override void LoadCore()
        {
            AssemblySource.Instance.Add(GetType().Assembly);
            base.LoadCore();
        }
    }
}
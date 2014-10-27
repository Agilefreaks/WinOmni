namespace OmniCommon
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Ninject.Extensions.Conventions;
    using Ninject.Modules;

    public abstract class ModuleBase : NinjectModule
    {
        #region Fields

        private IEnumerable<Type> _singletonTypes;

        #endregion

        #region Public Properties

        public virtual IEnumerable<Type> SingletonTypes
        {
            get
            {
                return _singletonTypes ?? (_singletonTypes = GenerateSingleTypesList());
            }
        }

        #endregion

        #region Public Methods and Operators

        public virtual void BindServices()
        {
            Kernel.Bind(
                configure =>
                configure.FromAssemblyContaining(GetType())
                    .Select(type => type.Name.EndsWith("Service"))
                    .BindDefaultInterface()
                    .Configure(c => c.InSingletonScope()));
        }

        public virtual void BindSingletons()
        {
            Kernel.Bind(
                configure =>
                configure.FromAssemblyContaining(GetType())
                    .Select(SingletonTypes.Contains)
                    .BindDefaultInterface()
                    .Configure(c => c.InSingletonScope()));
        }

        public virtual void BindViewModels()
        {
            Kernel.Bind(
                configure =>
                configure.FromAssemblyContaining(GetType()).Select(ShouldBindViewModel).BindDefaultInterface());
        }

        public override void Load()
        {
            ReplaceExistingBindings();

            BindSingletons();

            BindServices();

            BindViewModels();

            LoadCore();
        }

        public virtual void LoadCore()
        {
        }

        public virtual void ReplaceExistingBindings()
        {
        }

        #endregion

        #region Methods

        protected virtual IEnumerable<Type> GenerateSingleTypesList()
        {
            return Enumerable.Empty<Type>();
        }

        protected virtual bool ShouldBindViewModel(Type value)
        {
            return value.Name.EndsWith("ViewModel") && !SingletonTypes.Contains(value);
        }

        #endregion
    }
}
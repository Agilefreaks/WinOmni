namespace OmniUI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Caliburn.Micro;
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
                return _singletonTypes ?? (_singletonTypes = GenerateSingletonTypesList());
            }
        }

        #endregion

        #region Public Methods and Operators

        public override void Load()
        {
            AddModuleToViewSources();

            RemoveExistingBindings();

            BindSingletons();

            BindServices();

            BindViewModels();

            LoadCore();
        }

        #endregion

        #region Methods

        protected virtual void AddModuleToViewSources()
        {
            var currentAssemblyName = GetType().Assembly;
            if (!AssemblySource.Instance.Contains(currentAssemblyName))
            {
                AssemblySource.Instance.Add(currentAssemblyName);
            }
        }

        protected virtual void BindServices()
        {
            Kernel.Bind(
                configure =>
                configure.FromAssemblyContaining(GetType())
                    .Select(type => type.Name.EndsWith("Service"))
                    .BindDefaultInterface()
                    .Configure(c => c.InSingletonScope()));
        }

        protected virtual void BindSingletons()
        {
            Kernel.Bind(
                configure =>
                configure.FromAssemblyContaining(GetType())
                    .Select(SingletonTypes.Contains)
                    .BindDefaultInterface()
                    .Configure(c => c.InSingletonScope()));
        }

        protected virtual void BindViewModels()
        {
            Kernel.Bind(
                configure =>
                configure.FromAssemblyContaining(GetType()).Select(ShouldBindViewModel).BindDefaultInterface());
        }

        protected virtual IEnumerable<Type> GenerateSingletonTypesList()
        {
            return Enumerable.Empty<Type>();
        }

        protected virtual void LoadCore()
        {
        }

        protected virtual void RemoveExistingBindings()
        {
            foreach (var binding in TypesToOverriderBindingsFor().SelectMany(type => Kernel.GetBindings(type)))
            {
                Kernel.RemoveBinding(binding);
            }
        }

        protected virtual bool ShouldBindViewModel(Type value)
        {
            return value.Name.EndsWith("ViewModel") && !SingletonTypes.Contains(value);
        }

        protected virtual IEnumerable<Type> TypesToOverriderBindingsFor()
        {
            return Enumerable.Empty<Type>();
        }

        #endregion
    }
}
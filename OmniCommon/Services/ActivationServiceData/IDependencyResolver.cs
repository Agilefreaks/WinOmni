namespace OmniCommon.Services.ActivationServiceData
{
    using System;

    public interface IDependencyResolver
    {
        object Get(Type type, params DependencyParameter[] dependencyParameters);
    }
}
using System.Reflection;

namespace Commerce.Infrastructure;

public interface IExecutingFunctionContext
{
    public Type ExecutingAssembly { get; }

    public MethodInfo FunctionMethod { get; }

    public bool HasAuthRequirements { get; }
}
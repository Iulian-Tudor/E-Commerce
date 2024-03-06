using System.Reflection;

namespace Commerce.Infrastructure;

internal sealed class ExecutingFunctionContext : IExecutingFunctionContext
{
    public Type ExecutingAssembly { get; set; }

    public MethodInfo FunctionMethod { get; set; }

    public bool HasAuthRequirements { get; set; }
}
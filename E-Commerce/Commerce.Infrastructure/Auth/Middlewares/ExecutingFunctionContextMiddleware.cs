using System.Reflection;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace Commerce.Infrastructure;

internal sealed class ExecutingFunctionContextMiddleware<T> : IFunctionsWorkerMiddleware
{
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var executingAssembly = typeof(T);

        var lastSeparatorIndex = context.FunctionDefinition.EntryPoint.LastIndexOf(".");
        var functionTypePath = context.FunctionDefinition.EntryPoint[..lastSeparatorIndex];
        var methodName = context.FunctionDefinition.EntryPoint[(lastSeparatorIndex + 1)..];

        var functionType = executingAssembly.Assembly.GetType(functionTypePath);
        var executingMethod = functionType.GetMethod(methodName);

        var executingFunction = context.InstanceServices.GetRequiredService<ExecutingFunctionContext>();
        executingFunction.ExecutingAssembly = typeof(T);
        executingFunction.FunctionMethod = executingMethod;

        executingFunction.HasAuthRequirements = executingFunction.FunctionMethod.GetCustomAttribute<FunctionRequiresAuth>() != null;

        await next(context);
    }
}
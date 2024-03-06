namespace Commerce.Infrastructure;

[AttributeUsage(AttributeTargets.Method)]
public sealed class FunctionRequiresAuth : Attribute;
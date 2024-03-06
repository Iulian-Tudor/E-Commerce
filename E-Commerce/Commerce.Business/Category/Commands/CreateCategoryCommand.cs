using MediatR;
using CSharpFunctionalExtensions;

namespace Commerce.Business;

public sealed record CreateCategoryCommand(string Name, string Description) : IRequest<Result>;

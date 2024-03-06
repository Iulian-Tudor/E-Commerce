using MediatR;
using Commerce.Domain;
using CSharpFunctionalExtensions;

using Errors = Commerce.Business.BusinessErrors.Category.Create;

namespace Commerce.Business;

internal sealed class CreateCategoryCommandHandler(ICategoryRepository categoryRepository) : IRequestHandler<CreateCategoryCommand, Result>
{
    public async Task<Result> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        return await Result 
            .FailureIf(categoryRepository.Query().Any(c => c.Name == request.Name), Errors.CategoryAlreadyExists)
            .Bind(() => Category.Create(request.Name, request.Description))
            .Tap(categoryRepository.Store);
    }
}
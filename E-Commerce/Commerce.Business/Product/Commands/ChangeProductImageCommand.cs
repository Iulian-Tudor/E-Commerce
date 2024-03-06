using MediatR;
using Newtonsoft.Json;
using CSharpFunctionalExtensions;

namespace Commerce.Business;

public sealed record ChangeProductImageCommand(
    Guid ProductId,
    [property: JsonIgnore] Stream Stream,
    string FileName,
    string MimeType
) : IRequest<Result>;
namespace Commerce.SharedKernel.Domain;

public abstract class Document
{
    protected Document()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; protected set; }
}
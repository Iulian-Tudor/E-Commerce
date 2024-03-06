using CSharpFunctionalExtensions;

namespace Commerce.Domain;

public class FixedPrecisionPrice : ValueObject
{
    private const int Precision = 2;

    public FixedPrecisionPrice(decimal value)
    {
        Value = decimal.Round(value, Precision);
    }

    public decimal Value { get; private set; }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator decimal(FixedPrecisionPrice x) => x.Value;

    public static implicit operator FixedPrecisionPrice(decimal x) => new(x);
}

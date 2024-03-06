namespace Commerce.Domain;

public static class UserGatesFactory
{
    public static UserGate Any() => ForUser(Guid.NewGuid());

    public static UserGate ForUser(Guid userId) => UserGate.Create(userId).Value;

    public static UserGate Passed(this UserGate gate)
    {
        gate.Pass(gate.PassCode);

        return gate;
    }

    public static UserGate Exchanged(this UserGate gate)
    {
        gate.Passed().Exchange(gate.Secret);

        return gate;
    }
}
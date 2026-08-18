namespace DomainStateTransitions.Domain.Payments;

/// <summary>
/// Represents the lifecycle state of a payment and owns the rules that govern
/// which state transitions are valid.
/// </summary>
public sealed record PaymentStatus
{
    private PaymentStatus(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public static PaymentStatus Pending { get; } = new(nameof(Pending));

    public static PaymentStatus Accepted { get; } = new(nameof(Accepted));

    public static PaymentStatus Executing { get; } = new(nameof(Executing));

    public static PaymentStatus Succeeded { get; } = new(nameof(Succeeded));

    public static PaymentStatus Failed { get; } = new(nameof(Failed));

    public static PaymentStatus Unknown { get; } = new(nameof(Unknown));

    public bool CanTransitionTo(PaymentStatus nextStatus)
    {
        ArgumentNullException.ThrowIfNull(nextStatus);

        return (this, nextStatus) switch
        {
            _ when this == Pending && nextStatus == Accepted => true,
            _ when this == Accepted && nextStatus == Executing => true,
            _ when this == Executing && nextStatus is var next &&
                (next == Succeeded || next == Failed || next == Unknown) => true,
            _ when this == Unknown && nextStatus is var next &&
                (next == Succeeded || next == Failed) => true,
            _ => false
        };
    }

    public override string ToString() => Name;
}

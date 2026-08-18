using DomainStateTransitions.Domain.Exceptions;

namespace DomainStateTransitions.Domain.Payments;

/// <summary>
/// Aggregate root for the deliberately small payment lifecycle example.
///
/// The aggregate exposes business operations such as Accept and StartExecution,
/// while PaymentStatus owns the rules that decide whether each state transition
/// is valid.
/// </summary>
public sealed class Payment
{
    private Payment(Guid id)
    {
        Id = id;
        Status = PaymentStatus.Pending;
    }

    public Guid Id { get; private set; }

    public PaymentStatus Status { get; private set; }

    public static Payment Create()
    {
        return new Payment(Guid.NewGuid());
    }

    // --------------- COMMANDS

    public void Accept()
    {
        ChangeStatus(PaymentStatus.Accepted);
    }

    public void StartExecution()
    {
        ChangeStatus(PaymentStatus.Executing);
    }

    public void MarkAsSucceeded()
    {
        ChangeStatus(PaymentStatus.Succeeded);
    }

    public void MarkAsFailed()
    {
        ChangeStatus(PaymentStatus.Failed);
    }

    public void MarkOutcomeAsUnknown()
    {
        ChangeStatus(PaymentStatus.Unknown);
    }

    // --------------- PRIVATE VALIDATION AND HELPERS

    private void ChangeStatus(PaymentStatus nextStatus)
    {
        if (Status == nextStatus)
            return;

        if (!Status.CanTransitionTo(nextStatus))
        {
            throw new DomainException(
                $"Payment status cannot transition from {Status.Name} to {nextStatus.Name}.");
        }

        Status = nextStatus;
    }
}

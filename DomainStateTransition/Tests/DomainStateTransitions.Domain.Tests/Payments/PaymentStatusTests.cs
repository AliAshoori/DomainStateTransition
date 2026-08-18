using DomainStateTransitions.Domain.Payments;
using Xunit;

namespace DomainStateTransitions.Domain.Tests.Payments;

public sealed class PaymentStatusTests
{
    public static TheoryData<PaymentStatus, PaymentStatus> ValidTransitions => new()
    {
        { PaymentStatus.Pending, PaymentStatus.Accepted },
        { PaymentStatus.Accepted, PaymentStatus.Executing },
        { PaymentStatus.Executing, PaymentStatus.Succeeded },
        { PaymentStatus.Executing, PaymentStatus.Failed },
        { PaymentStatus.Executing, PaymentStatus.Unknown },
        { PaymentStatus.Unknown, PaymentStatus.Succeeded },
        { PaymentStatus.Unknown, PaymentStatus.Failed }
    };

    public static TheoryData<PaymentStatus, PaymentStatus> InvalidTransitions => new()
    {
        { PaymentStatus.Pending, PaymentStatus.Succeeded },
        { PaymentStatus.Pending, PaymentStatus.Failed },
        { PaymentStatus.Accepted, PaymentStatus.Succeeded },
        { PaymentStatus.Succeeded, PaymentStatus.Executing },
        { PaymentStatus.Failed, PaymentStatus.Accepted },
        { PaymentStatus.Unknown, PaymentStatus.Executing }
    };

    [Theory]
    [MemberData(nameof(ValidTransitions))]
    public void CanTransitionTo_ShouldAllowDefinedLifecycleTransitions(
        PaymentStatus current,
        PaymentStatus next)
    {
        Assert.True(current.CanTransitionTo(next));
    }

    [Theory]
    [MemberData(nameof(InvalidTransitions))]
    public void CanTransitionTo_ShouldRejectUndefinedLifecycleTransitions(
        PaymentStatus current,
        PaymentStatus next)
    {
        Assert.False(current.CanTransitionTo(next));
    }
}

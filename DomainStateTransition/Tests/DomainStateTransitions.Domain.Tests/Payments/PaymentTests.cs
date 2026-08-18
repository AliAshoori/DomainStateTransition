using DomainStateTransitions.Domain.Exceptions;
using DomainStateTransitions.Domain.Payments;
using Xunit;

namespace DomainStateTransitions.Domain.Tests.Payments;

public sealed class PaymentTests
{
    [Fact]
    public void Create_ShouldCreatePendingPayment()
    {
        var payment = Payment.Create();

        Assert.NotEqual(Guid.Empty, payment.Id);
        Assert.Equal(PaymentStatus.Pending, payment.Status);
    }

    [Fact]
    public void Payment_ShouldFollowHappyPathLifecycle()
    {
        var payment = Payment.Create();

        payment.Accept();
        Assert.Equal(PaymentStatus.Accepted, payment.Status);

        payment.StartExecution();
        Assert.Equal(PaymentStatus.Executing, payment.Status);

        payment.MarkAsSucceeded();
        Assert.Equal(PaymentStatus.Succeeded, payment.Status);
    }

    [Fact]
    public void Payment_ShouldRejectInvalidTransition()
    {
        var payment = Payment.Create();

        var exception = Assert.Throws<DomainException>(payment.MarkAsSucceeded);

        Assert.Equal(
            "Payment status cannot transition from Pending to Succeeded.",
            exception.Message);
        Assert.Equal(PaymentStatus.Pending, payment.Status);
    }

    [Fact]
    public void ExecutingPayment_ShouldAllowUnknownOutcome()
    {
        var payment = Payment.Create();
        payment.Accept();
        payment.StartExecution();

        payment.MarkOutcomeAsUnknown();

        Assert.Equal(PaymentStatus.Unknown, payment.Status);
    }

    [Fact]
    public void UnknownPayment_ShouldAllowReconciliationToSucceeded()
    {
        var payment = CreateUnknownPayment();

        payment.MarkAsSucceeded();

        Assert.Equal(PaymentStatus.Succeeded, payment.Status);
    }

    [Fact]
    public void UnknownPayment_ShouldAllowReconciliationToFailed()
    {
        var payment = CreateUnknownPayment();

        payment.MarkAsFailed();

        Assert.Equal(PaymentStatus.Failed, payment.Status);
    }

    private static Payment CreateUnknownPayment()
    {
        var payment = Payment.Create();
        payment.Accept();
        payment.StartExecution();
        payment.MarkOutcomeAsUnknown();

        return payment;
    }
}

# Domain State Transitions

A deliberately small DDD example exploring when a domain status can deserve richer modelling than a passive enum.

The sample uses a simplified payment lifecycle:

```text
Pending -> Accepted -> Executing -> Succeeded
                            |-----> Failed
                            |-----> Unknown -> Succeeded
                                         |-> Failed
```

The key design choice is the separation of responsibilities:

- `Payment` is the aggregate and exposes business operations such as `Accept()`, `StartExecution()` and `MarkAsSucceeded()`.
- `PaymentStatus` is an immutable value object that owns the rules governing valid state transitions.

The repository intentionally contains no API, persistence, messaging, payment-provider integration or infrastructure. Its purpose is to keep the discussion focused on one domain-modelling decision and its trade-offs.

## Lifecycle semantics

For this deliberately simplified model:

- `Pending` means the payment exists but has not yet been accepted for execution.
- `Accepted` means the system has accepted responsibility for attempting execution.
- `Executing` means execution is in progress.
- `Succeeded` and `Failed` are known terminal outcomes.
- `Unknown` represents an ambiguous execution outcome, such as a downstream timeout after the payment instruction may already have been processed. Reconciliation can later resolve `Unknown` to `Succeeded` or `Failed`.

These states are intentionally simplified for the modelling discussion; they are not intended to represent the complete lifecycle of every real-world payment rail.

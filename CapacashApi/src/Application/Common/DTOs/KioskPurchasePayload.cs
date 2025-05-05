namespace Capacash.Application.Commons.DTOs{
public record KioskPurchasePayload(
    string KioskId,
    decimal Amount,
    DateTime Timestamp);
}

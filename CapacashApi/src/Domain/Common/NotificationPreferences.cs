// Capacash.Domain.Common/NotificationPreferences.cs
namespace Capacash.Domain.Common
{
    public record NotificationPreferences(
        bool TransactionEmail,
        bool TransactionPush,
        bool SecuritySms,
        bool PromotionalEmail
    );
}
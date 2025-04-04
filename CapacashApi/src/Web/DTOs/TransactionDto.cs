using System.ComponentModel.DataAnnotations;

namespace Capacash.Web.Models
{
    public class TransactionDto
{
    [Required]
    public Guid UserId { get; set; }  // ✅ Use Guid directly, not string

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }
}

}

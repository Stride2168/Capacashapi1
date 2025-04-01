
namespace Capacash.Domain.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }

        public Employee? Employee { get; set; }
    }
}

namespace Capacash.Application.Commons.DTOs
{
    public record WalletDto
    {
        public Guid Id { get; }
        public decimal Balance { get; }
        public Guid UserId { get; }
        public DateTime CreatedAt { get; }
        public string FullName { get; }  // Add FullName property

        public WalletDto(Guid id, decimal balance, Guid userId, DateTime createdAt, string fullName)
        {
            Id = id;
            Balance = balance;
            UserId = userId;
            CreatedAt = createdAt;
            FullName = fullName;  // Initialize FullName
        }
    }
}

using MediatR;
using System;
using System.Collections.Generic;
using Capacash.Application.Commons.DTOs;

namespace Capacash.Application.Transactions.Queries
{
    public class GetTransactionsQuery : IRequest<List<TransactionDto>>
    {
        public string CompanyId { get; }
        public Guid? UserId { get; }
        public string? TransactionType { get; }
        public DateTime? StartDate { get; }
        public DateTime? EndDate { get; }
        public string? DateRange { get; }

        public GetTransactionsQuery(
    string companyId,
    Guid? userId = null,
    string? transactionType = null,
    DateTime? startDate = null,
    DateTime? endDate = null,
    string? dateRange = null)
{
    CompanyId = companyId ?? throw new ArgumentNullException(nameof(companyId));
    UserId = userId;
    TransactionType = transactionType;
    StartDate = startDate;
    EndDate = endDate;
    DateRange = dateRange;
}

        public void Deconstruct(
            out string companyId,
            out Guid? userId,
            out string? transactionType,
            out DateTime? startDate,
            out DateTime? endDate,
            out string? dateRange)
        {
            companyId = CompanyId;
            userId = UserId;
            transactionType = TransactionType;
            startDate = StartDate;
            endDate = EndDate;
            dateRange = DateRange;
        }
    }
}

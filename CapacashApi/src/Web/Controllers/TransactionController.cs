using Capacash.Application.Transactions.Queries;
using Capacash.Application.Transactions.Queries.GetAllTransactions;
using Capacash.Application.Transactions.Queries.GetMonthlyTransactionSummary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Capacash.WebAPI.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class TransactionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransactionController(IMediator mediator)
        {
            _mediator = mediator;
        }

      [HttpGet("all")]
public async Task<IActionResult> GetAllCompanyTransactions()
{
    var companyId = User.FindFirst("CompanyId")?.Value;
    if (string.IsNullOrWhiteSpace(companyId))
        return Unauthorized(new { Error = "Company ID is missing in the token." });

    var query = new GetAllTransactionsQuery(companyId);
    var result = await _mediator.Send(query);
    return Ok(result);
}
[HttpGet("summary/monthly")]
public async Task<IActionResult> GetMonthlyTransactionSummary()
{
    var companyId = User.FindFirst("CompanyId")?.Value;
    if (string.IsNullOrWhiteSpace(companyId))
        return Unauthorized(new { Error = "Company ID is missing in the token." });

    var query = new GetMonthlyTransactionSummaryQuery(companyId);
    var result = await _mediator.Send(query);
    return Ok(result);
}

    }
}

using MediatR;
using System.Collections.Generic;
using Capacash.Application.Commons.DTOs;
namespace Capacash.Application.Users.Queries.GetUnapprovedUsers{

public record GetUnapprovedUsersQuery(string CompanyId) : IRequest<List<UnapprovedUserDto>>;}

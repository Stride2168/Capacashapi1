using Capacash.Domain.Entities;
using MediatR;
using System.Collections.Generic;

namespace Capacash.Application.SuperAdmin.Queries;

public record GetPendingAdminsQuery : IRequest<List<User>>;

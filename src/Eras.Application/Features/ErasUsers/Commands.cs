using Eras.Application.DTOs.UsersManagement;

using MediatR;

namespace Eras.Application.Features.ErasUsers;

public sealed record CreateErasUserCommand(ErasUserDTO ErasUser) : IRequest<ErasUserDTO>;
public sealed record UpdateErasUserCommand(ErasUserDTO ErasUser) : IRequest<ErasUserDTO>;
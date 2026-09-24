using Eras.Application.DTOs.UsersManagement;

using MediatR;

namespace Eras.Application.Features.ErasUsers;

public sealed record GetErasUserCheckByEmailQuery(string Email) : IRequest<ErasUserDTO?>;
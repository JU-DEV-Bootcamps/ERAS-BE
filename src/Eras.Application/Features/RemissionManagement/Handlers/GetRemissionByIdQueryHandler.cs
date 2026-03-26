

using Eras.Application.Contracts.Persistence.RemissionManagement;
using Eras.Application.DTOs.RemissionManagement;
using Eras.Application.Mappers.RemissionManagement;
using Eras.Domain.Entities.RemissionsManagement;

using MediatR;

namespace Eras.Application.Features.RemissionManagement.Handlers;

public sealed class GetRemissionByIdQueryHandler
    : IRequestHandler<GetRemissionByIdQuery, RemissionDto?>
{
    private readonly IRemissionRepository _repository;
    private readonly IMapper<Remission, RemissionDto> _mapper;

    public GetRemissionByIdQueryHandler(
        IRemissionRepository repository,
        IMapper<Remission, RemissionDto> mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<RemissionDto?> Handle(
        GetRemissionByIdQuery request,
        CancellationToken cancellationToken)
    {
        Remission? entity = await _repository.GetByIdAsync(request.Id);

        return entity is null ? null : _mapper.Map(entity);
    }
}
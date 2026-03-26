

using Eras.Application.Contracts.Persistence.RemissionManagement;
using Eras.Application.DTOs.RemissionManagement;
using Eras.Application.Mappers.RemissionManagement;
using Eras.Domain.Entities.RemissionsManagement;

using MediatR;

namespace Eras.Application.Features.RemissionManagement.Handlers;

public sealed class GetRemissionsByStatusQueryHandler
    : IRequestHandler<GetRemissionsByStatusQuery, IReadOnlyCollection<RemissionDto>>
{
    private readonly IRemissionRepository _repository;
    private readonly IMapper<Remission, RemissionDto> _mapper;

    public GetRemissionsByStatusQueryHandler(
        IRemissionRepository repository,
        IMapper<Remission, RemissionDto> mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyCollection<RemissionDto>> Handle(
        GetRemissionsByStatusQuery request,
        CancellationToken cancellationToken)
    {
        IEnumerable<Remission> entities = await _repository.GetByStatusAsync(request.Status);

        return entities.Select(_mapper.Map).ToArray();
    }
}
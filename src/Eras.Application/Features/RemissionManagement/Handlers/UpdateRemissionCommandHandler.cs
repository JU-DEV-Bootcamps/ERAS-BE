

using Eras.Application.Contracts.Persistence.RemissionManagement;
using Eras.Application.DTOs.RemissionManagement;
using Eras.Application.Mappers.RemissionManagement;
using Eras.Application.Validation;
using Eras.Domain.Entities.RemissionsManagement;

using FluentValidation;

using MediatR;

namespace Eras.Application.Features.RemissionManagement.Handlers;

public sealed class UpdateRemissionCommandHandler
    : IRequestHandler<UpdateRemissionCommand, RemissionDto>
{
    private readonly IMapper<RemissionDto, Remission> _toDomainMapper;
    private readonly IMapper<Remission, RemissionDto> _toDtoMapper;
    private readonly IValidator<Remission> _validator;
    private readonly IRemissionRepository _repository;

    public UpdateRemissionCommandHandler(
        IMapper<RemissionDto, Remission> toDomainMapper,
        IMapper<Remission, RemissionDto> toDtoMapper,
        IValidator<Remission> validator,
        IRemissionRepository repository)
    {
        _toDomainMapper = toDomainMapper;
        _toDtoMapper = toDtoMapper;
        _validator = validator;
        _repository = repository;
    }

    public async Task<RemissionDto> Handle(
        UpdateRemissionCommand request,
        CancellationToken cancellationToken)
    {
        Remission entity = _toDomainMapper.Map(request.Remission);

        await ValidationHelper.ValidateAndThrowAsync(_validator, entity, cancellationToken);

        Remission persisted = await _repository.UpdateAsync(entity);

        return _toDtoMapper.Map(persisted);
    }
}
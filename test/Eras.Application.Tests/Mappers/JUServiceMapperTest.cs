using Eras.Application.DTOs;
using Eras.Application.Mappers;
using Eras.Domain.Common;
using Eras.Domain.Entities;

namespace Eras.Application.Tests.Mappers;

public class JUServiceMapperTest
{
    [Fact]
    public void ToDomain_Should_Map_Properties()
    {
        var audit = new AuditInfo();

        var dto = new JUServiceDTO
        {
            Id = 1,
            Name = "Service",
            Audit = audit
        };

        var result = dto.ToDomain();

        Assert.Equal(dto.Id, result.Id);
        Assert.Equal(dto.Name, result.Name);
        Assert.Same(audit, result.Audit);
    }

    [Fact]
    public void ToDomain_Should_Throw_When_Dto_Is_Null()
    {
        JUServiceDTO dto = null!;

        Assert.Throws<ArgumentNullException>(() => dto.ToDomain());
    }

    [Fact]
    public void ToDomain_Should_Throw_When_Audit_Is_Null()
    {
        var dto = new JUServiceDTO
        {
            Id = 1,
            Name = "Service",
            Audit = null
        };

        var exception = Assert.Throws<ArgumentException>(() => dto.ToDomain());

        Assert.Equal("Audit is required. (Parameter 'Dto')", exception.Message);
    }

    [Fact]
    public void ToDTO_Should_Map_Properties()
    {
        var audit = new AuditInfo();

        var entity = new JUService
        {
            Id = 1,
            Name = "Service",
            Audit = audit
        };

        var result = entity.ToDTO();

        Assert.Equal(entity.Id, result.Id);
        Assert.Equal(entity.Name, result.Name);
        Assert.Same(audit, result.Audit);
    }

    [Fact]
    public void ToDTO_Should_Throw_When_Entity_Is_Null()
    {
        JUService entity = null!;

        Assert.Throws<ArgumentNullException>(() => entity.ToDTO());
    }

    [Fact]
    public void ToDTO_Should_Throw_When_Audit_Is_Null()
    {
        var entity = new JUService
        {
            Id = 1,
            Name = "Service",
            Audit = null!
        };

        var exception = Assert.Throws<ArgumentException>(() => entity.ToDTO());

        Assert.Equal("Audit is required. (Parameter 'Entity')", exception.Message);
    }
}

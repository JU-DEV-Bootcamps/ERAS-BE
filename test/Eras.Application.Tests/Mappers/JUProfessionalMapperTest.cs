using Eras.Application.DTOs;
using Eras.Application.Mappers;
using Eras.Domain.Common;
using Eras.Domain.Entities;

namespace Eras.Application.Tests.Mappers;

public class JUProfessionalMapperTest
{
    [Fact]
    public void ToDomain_Should_Map_Properties()
    {
        var audit = new AuditInfo();

        var dto = new JUProfessionalDTO
        {
            Id = 1,
            Name = "Anne",
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
        JUProfessionalDTO dto = null!;

        Assert.Throws<ArgumentNullException>(() => dto.ToDomain());
    }

    [Fact]
    public void ToDomain_Should_Throw_When_Audit_Is_Null()
    {
        var dto = new JUProfessionalDTO
        {
            Id = 1,
            Name = "Bea",
            Audit = null
        };

        var exception = Assert.Throws<ArgumentException>(() => dto.ToDomain());

        Assert.Equal("Audit is required. (Parameter 'Dto')", exception.Message);
    }

    [Fact]
    public void ToDTO_Should_Map_Properties()
    {
        var audit = new AuditInfo();

        var entity = new JUProfessional
        {
            Id = 1,
            Name = "Anne",
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
        JUProfessional entity = null!;

        Assert.Throws<ArgumentNullException>(() => entity.ToDTO());
    }

    [Fact]
    public void ToDTO_Should_Throw_When_Audit_Is_Null()
    {
        var entity = new JUProfessional
        {
            Id = 1,
            Name = "Bea",
            Audit = null!
        };

        var exception = Assert.Throws<ArgumentException>(() => entity.ToDTO());

        Assert.Equal("Audit is required. (Parameter 'Entity')", exception.Message);
    }
}

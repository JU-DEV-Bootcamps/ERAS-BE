
using Eras.Application.DTOs;
using Eras.Application.Mappers;
using Eras.Domain.Common;
using Eras.Domain.Entities;

namespace Eras.Application.Tests.Mappers;

public class JUInterventionMapperTest
{
    [Fact]
    public void ToDomain_Should_Map_Properties()
    {
        var audit = new AuditInfo();

        var dto = new JUInterventionDTO
        {
            Id = 1,
            Diagnostic = "Intervention",
            Audit = audit,
            Objective = "Some",
            StudentId = 1,
        };

        var result = dto.ToDomain();

        Assert.Equal(dto.Id, result.Id);
        Assert.Equal(dto.Diagnostic, result.Diagnostic);
        Assert.Same(audit, result.Audit);
    }

    [Fact]
    public void ToDomain_Should_Throw_When_Dto_Is_Null()
    {
        JUInterventionDTO dto = null!;

        Assert.Throws<ArgumentNullException>(() => dto.ToDomain());
    }

    //[Fact]
    //public void ToDomain_Should_Throw_When_Audit_Is_Null()
    //{
    //    var dto = new JUInterventionDTO
    //    {
    //        Id = 1,
    //        Diagnostic = "Intervention",
    //        Audit = null!,
    //        Objective = "Some",
    //        StudentId = 1,
    //    };

    //    var exception = Assert.Throws<ArgumentException>(() => dto.ToDomain());

    //    Assert.Equal("Audit is required. (Parameter 'Dto')", exception.Message);
    //}

    [Fact]
    public void ToDTO_Should_Map_Properties()
    {
        var audit = new AuditInfo();

        var entity = new JUIntervention
        {
            Id = 1,
            Diagnostic = "Intervention",
            Audit = audit
        };

        var result = entity.ToDTO();

        Assert.Equal(entity.Id, result.Id);
        Assert.Equal(entity.Diagnostic, result.Diagnostic);
        Assert.Same(audit, result.Audit);
    }

    [Fact]
    public void ToDTO_Should_Throw_When_Entity_Is_Null()
    {
        JUIntervention entity = null!;

        Assert.Throws<ArgumentNullException>(() => entity.ToDTO());
    }
}

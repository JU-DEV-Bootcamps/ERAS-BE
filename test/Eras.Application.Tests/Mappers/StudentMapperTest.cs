using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.DTOs;
using Eras.Domain.Entities;
using Eras.Application.Mappers;

namespace Eras.Application.Tests.Mappers;
public class StudentMapperTest
{
    [Fact]
    public void ToDomain_Should_Convert_StudentDTO_To_Student()
    {
        var dto = new StudentDTO()
        {
            Uuid = "Uuid",
            Name = "Name",
            Email = "Email",
            IsImported = true,
        };
        var result = dto.ToDomain();
        Assert.NotNull(result);
        Assert.Equal(dto.Uuid, result.Uuid);
        Assert.Equal(dto.Name, result.Name);
        Assert.Equal(dto.Email, result.Email);
        Assert.Equal(dto.IsImported, result.IsImported);
    }

    [Fact]
    public void ToDto_Should_Convert_PollVersion_To_PollVersionDto()
    {
        var dto = new Student()
        {
            Uuid = "Uuid",
            Name = "Name",
            Email = "Email",
            IsImported = true,
        };
        var result = dto.ToDto();
        Assert.NotNull(result);
        Assert.Equal(dto.Uuid, result.Uuid);
        Assert.Equal(dto.Name, result.Name);
        Assert.Equal(dto.Email, result.Email);
        Assert.Equal(dto.IsImported, result.IsImported);
    }
}

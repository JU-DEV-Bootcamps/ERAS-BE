using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Students.Queries.GetAll;
using Eras.Application.Features.Students.Queries.GetAllByPollAndDate;
using Eras.Application.Utils;
using Eras.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Eras.Application.Tests.Features.Students.Queries;
public class GetAllStudentsByPollUuidAndDaysQueryHandlerTest
{
    private readonly Mock<IStudentRepository> _mockStudentRepository;
    private readonly Mock<ILogger<GetAllStudentsByPollUuidAndDaysQueryHandler>> _mockLogger;
    private readonly GetAllStudentsByPollUuidAndDaysQueryHandler _handler;

    public GetAllStudentsByPollUuidAndDaysQueryHandlerTest()
    {
        _mockStudentRepository = new Mock<IStudentRepository>();
        _mockLogger = new Mock<ILogger<GetAllStudentsByPollUuidAndDaysQueryHandler>>();
        _handler = new GetAllStudentsByPollUuidAndDaysQueryHandler(_mockStudentRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_ResponseAsync()
    {
        // Arrange
        var query = new GetAllStudentsByPollUuidAndDaysQuery()
        {
            Query = new Utils.Pagination(),
            PollUuid = "1",
            Days = 1
        };
        List<Student> students = new List<Student>()
            {
                new Student(){Email = "StudentEmail1",},
                new Student(){Email = "StudentEmail2"}
            };
        var response = (students,2);
        _mockStudentRepository
            .Setup(Repo => Repo.GetAllStudentsByPollUuidAndDaysQuery(It.IsAny<int>(), It.IsAny<int>(),It.IsAny<string>(), It.IsAny<int?>())).ReturnsAsync(response);

        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Items.Count);
    }
}

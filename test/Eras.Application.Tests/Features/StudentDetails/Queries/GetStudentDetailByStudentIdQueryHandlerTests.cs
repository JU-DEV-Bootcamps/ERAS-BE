using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.StudentsDetails.Queries.GetStudentDetailByStudentId;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.StudentDetails.Queries;

public class GetStudentDetailByStudentIdQueryHandlerTests
{
    private readonly Mock<IStudentDetailRepository> _studentDetailRepository;
    private readonly Mock<ILogger<GetStudentDetailByStudentIdQueryHandler>> _logger;
    private readonly GetStudentDetailByStudentIdQueryHandler _handler;

    public GetStudentDetailByStudentIdQueryHandlerTests()
    {
        _studentDetailRepository = new Mock<IStudentDetailRepository>();
        _logger = new Mock<ILogger<GetStudentDetailByStudentIdQueryHandler>>();
        _handler = new GetStudentDetailByStudentIdQueryHandler(
            _studentDetailRepository.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenStudentDetailRepositoryThrowsException()
    {
        var request = new GetStudentDetailByStudentIdQuery()
        {
            StudentId = 1
        };
        _studentDetailRepository
            .Setup(x => x.GetByStudentId(request.StudentId))
            .ThrowsAsync(new Exception());

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.Null(result.Body);
        Assert.Equal("Error", result.Message);
        Assert.False(result.Success);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenStudentDetailRepositoryReturnsNull()
    {
        var request = new GetStudentDetailByStudentIdQuery()
        {
            StudentId = 1
        };
        _studentDetailRepository
            .Setup(x => x.GetByStudentId(request.StudentId))
            .ReturnsAsync((StudentDetail?)null!);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.Null(result.Body);
        Assert.Equal("Student Detail doesn't exist", result.Message);
        Assert.False(result.Success);
    }

    [Fact]
    public async Task Handle_ShouldGetStudentDetailsByStudentId()
    {
        var request = new GetStudentDetailByStudentIdQuery()
        {
            StudentId = 1
        };

        var response = new StudentDetail
        {
            StudentId = 1,
            EnrolledCourses = 2,
            AvgScore = 73,
        };

        _studentDetailRepository
            .Setup(x => x.GetByStudentId(request.StudentId))
            .ReturnsAsync(response);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.Equal(response, result.Body);
        Assert.Equal("Success", result.Message);
        Assert.True(result.Success);
    }
}

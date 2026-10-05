using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Error.Bussiness;

using MediatR;

namespace Eras.Application.Features.Students.Commands.DeleteStudent;

public sealed class DeleteStudentCommandHandler(
    IStudentRepository StudentRepository,
    IStudentProfileRepository ProfileRepository)
    : IRequestHandler<DeleteStudentCommand>
{
    public async Task Handle(DeleteStudentCommand Request, CancellationToken CancellationToken)
    {
        Student student = await StudentRepository.GetByIdAsync(Request.StudentId)
            ?? throw new NotFoundException($"Student with ID {Request.StudentId} not found.");

        if (student.IsImported)
            throw new BussinessException("Imported students cannot be deleted.", 409);

        if (await ProfileRepository.GetByStudentIdAsync(student.Id) is null)
            throw new BussinessException("Only students created manually can be deleted.", 409);

        if (await StudentRepository.HasRelatedDataAsync(student.Id))
            throw new BussinessException(
                "This student already has answers or assessments and cannot be deleted.", 409);

        await StudentRepository.DeleteByIdAsync(student.Id);
    }
}

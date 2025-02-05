using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Mappers;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Students.Commands.CreateStudent
{
    public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, bool>
    {
        private readonly IStudentRepository _studentRepository;
        
        public CreateStudentCommandHandler(IStudentRepository studentRepository)
        {            
            _studentRepository = studentRepository;
        }

        public async Task<bool> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _studentRepository.AddAsync(request.student);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

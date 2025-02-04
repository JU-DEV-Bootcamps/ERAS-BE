using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.DTOs;
using Eras.Application.Events;
using Eras.Domain.Repositories;
using MediatR;

namespace Eras.Application.Handlers
{
    public class CreateStudentsHandler : IRequestHandler<CreateStudentsEvent, bool>
    {

        private IStudentRepository _studentRepository;

        public CreateStudentsHandler(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public Task<bool> Handle(CreateStudentsEvent request, CancellationToken cancellationToken)
        {
            try
            {
                _studentRepository.SaveAsync(request.student);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                return Task.FromResult(false);
            }
        }
    }
}

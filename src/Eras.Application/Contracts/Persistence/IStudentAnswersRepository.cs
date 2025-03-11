using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface IStudentAnswersRepository
    {
        Task<List<StudentAnswer>> GetStudentAnswersAsync(int studentId, int pollId);
    }
}

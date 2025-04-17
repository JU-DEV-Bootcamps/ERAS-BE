using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Students.Commands.CreateStudent;
using Eras.Application.Mappers;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Common;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Features.StudentsDetails.Commands.CreateStudentDetail
{
    public class CreateStudentDetailCommandHandler : IRequestHandler<CreateStudentDetailCommand, CreateCommandResponse<StudentDetail>>
    {
        private readonly IStudentDetailRepository _studentDetailRepository;
        private readonly ILogger<CreateStudentDetailCommandHandler> _logger;

        public CreateStudentDetailCommandHandler(IStudentDetailRepository studentDetailRepository, 
            ILogger<CreateStudentDetailCommandHandler> logger)
        {
            _studentDetailRepository = studentDetailRepository;
            _logger = logger;
        }


        public async Task<CreateCommandResponse<StudentDetail>> Handle(CreateStudentDetailCommand request, CancellationToken cancellationToken)
        {

            try
            {
                StudentDetail response = await _studentDetailRepository.GetByStudentId(request.StudentDetailDto.StudentId);
                StudentDetail studentDetail = request.StudentDetailDto.ToDomain();
                if (response != null)
                {
                    response.EnrolledCourses = studentDetail.EnrolledCourses;
                    response.GradedCourses = studentDetail.GradedCourses;
                    response.TimeDeliveryRate = studentDetail.TimeDeliveryRate;
                    response.AvgScore = studentDetail.AvgScore;
                    response.CoursesUnderAvg = studentDetail.CoursesUnderAvg;
                    response.PureScoreDiff = studentDetail.PureScoreDiff;
                    response.StandardScoreDiff = studentDetail.StandardScoreDiff;
                    response.LastAccessDays = studentDetail.LastAccessDays;
                    response.Audit = studentDetail.Audit;
                    response.Audit.ModifiedAt = DateTime.UtcNow;
                    StudentDetail existentStudentDetail = await _studentDetailRepository.UpdateAsync(response);
                    return new CreateCommandResponse<StudentDetail>(existentStudentDetail, 0, "Success", true);
                }
                else
                {
                    StudentDetail studentDetailCreated = await _studentDetailRepository.AddAsync(studentDetail);
                    return new CreateCommandResponse<StudentDetail>(studentDetailCreated, 1, "Success", true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred creating student detail {request.StudentDetailDto}: {ex.Message}");
                return new CreateCommandResponse<StudentDetail>(null, 0, "Error", false);
            }
        }
    }
}

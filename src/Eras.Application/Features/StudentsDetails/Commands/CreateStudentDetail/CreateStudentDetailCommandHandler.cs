﻿using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Students.Commands.CreateStudent;
using Eras.Application.Mappers;
using Eras.Application.Models;
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
    public class CreateStudentDetailCommandHandler : IRequestHandler<CreateStudentDetailCommand, CreateComandResponse<StudentDetail>>
    {
        private readonly IStudentDetailRepository _studentDetailRepository;
        private readonly ILogger<CreateStudentDetailCommandHandler> _logger;

        public CreateStudentDetailCommandHandler(IStudentDetailRepository studentDetailRepository, ILogger<CreateStudentDetailCommandHandler> logger)
        {
            _studentDetailRepository = studentDetailRepository;
            _logger = logger;
        }


        public async Task<CreateComandResponse<StudentDetail>> Handle(CreateStudentDetailCommand request, CancellationToken cancellationToken)
        {

            try
            {
                StudentDetail response = await _studentDetailRepository.GetByStudentId(request.StudentDetailDto.StudentId);
                if (response != null)
                    return new CreateComandResponse<StudentDetail>(response, 0, "Success", true);
                StudentDetail studentDetail = request.StudentDetailDto.ToDomain();
                StudentDetail studentDetailCreated = await _studentDetailRepository.AddAsync(studentDetail); 
                return new CreateComandResponse<StudentDetail>(studentDetailCreated, 1, "Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred creating student detail {request.StudentDetailDto}: {ex.Message}");
                return new CreateComandResponse<StudentDetail>(null, 0, "Error", false);
            }
        }
    }
}
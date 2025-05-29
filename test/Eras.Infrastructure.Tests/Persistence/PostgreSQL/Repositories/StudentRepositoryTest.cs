using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.Mappers;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories
{   

    public class StudentRepositoryTest
    {
        private AppDbContext _context;
        private StudentRepository _repository;

        public StudentRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "StudentTest")
                .Options;
            _context = new AppDbContext(options);
            _context.Database.EnsureCreated();
            _repository = new StudentRepository(_context);
        }


        [Fact]
        public async void UpdateShouldUpdateSuccessfullyAsync()
        {
            StudentDetail studentDetail = new StudentDetail() {
                   AvgScore = 1,
            };
            Student student = new Student() {
                Name = "Test",
            };
            student.StudentDetail = studentDetail;
            var resultCreated = await _repository.AddAsync(student);
            resultCreated.Name = "TestUpdate";
            var resultUpdated = await _repository.UpdateAsync(resultCreated);
            Assert.NotNull(resultUpdated);
            Assert.Equal("TestUpdate", resultUpdated.Name);
        }

    }
}

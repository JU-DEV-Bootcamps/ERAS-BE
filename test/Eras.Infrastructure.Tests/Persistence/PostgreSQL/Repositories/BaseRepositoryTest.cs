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

    public class BaseRepositoyTest
    {
        private AppDbContext _context;
        private BaseRepository<Poll, PollEntity> _repository;

        public BaseRepositoyTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "UserServiceTest")
                .Options;
            _context = new AppDbContext(options);
            _context.Database.EnsureCreated();
            _repository = new BaseRepository<Poll, PollEntity>(
                _context,
                entity => entity.ToDomain(),
                model => model.ToPersistence()
            );
        }


        [Fact]
        public async void GetByLastDays_Should_Return()
        {
            Poll poll = new Poll
            {
                Name = "New Poll Name",
            };
            var result = await _repository.AddAsync(poll);
            Assert.NotNull(result);
            Assert.Equal(result.Name, poll.Name);
        }

    }
}

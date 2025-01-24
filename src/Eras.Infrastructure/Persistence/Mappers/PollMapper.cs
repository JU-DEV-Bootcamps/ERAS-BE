using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Infrastructure.Persistence.Mappers
{
    public static class PollMapper
    {
        public static PollsEntity ToPollEntity(this Poll poll)
        {
            if (poll == null) throw new ArgumentNullException(nameof(poll));
            return new PollsEntity
            {
                Id = poll.Id,
                Name = poll.PollName,
                CreatedDate = poll.CreatedDate.ToUniversalTime(),
                ModifiedDate = poll.ModifiedDate.ToUniversalTime()
            };
        }
    }
}
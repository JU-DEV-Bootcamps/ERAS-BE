using Eras.Application.DTOs;
using Eras.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Dtos
{
    public class PollDTO
    {
        private string _id = String.Empty; //inventoryId

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Name { get; set; } = string.Empty; //name
        public string? Version { get; set; }
        public ICollection<ComponentDTO> Components { get; set; } = [];
    }
   }

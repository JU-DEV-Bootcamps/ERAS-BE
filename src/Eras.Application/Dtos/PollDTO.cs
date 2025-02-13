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
        public int Id { get; set; }

        private string _idCosmicLatte = String.Empty;

        public string IdCosmicLatte
        {
            get { return _idCosmicLatte; }
            set { _idCosmicLatte = value; }
        }

        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public ICollection<ComponentDTO> Components { get; set; } = [];
    }
   }

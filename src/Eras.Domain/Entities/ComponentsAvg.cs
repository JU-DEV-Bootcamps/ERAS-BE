﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Domain.Entities
{
    public class ComponentsAvg
    {
        public int PollId { get; set; }
        public int ComponentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public float ComponentAvg { get; set; }
    }
}

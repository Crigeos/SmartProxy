﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Movie : MongoDocument
    {
        public string name { get; set; }
        public List<string> Actors { get; set; }
        public decimal? Budget { get; set; }
        public string Description { get; set; }
    }
}

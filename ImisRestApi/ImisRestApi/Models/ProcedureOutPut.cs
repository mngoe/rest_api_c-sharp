﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ImisRestApi.Models
{
    public class ProcedureOutPut
    {
        public int Code { get; set; }
        public DataTable Data { get; set; }
    }
}

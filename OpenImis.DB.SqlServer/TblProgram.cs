using System;
using System.Collections.Generic;

namespace OpenImis.DB.SqlServer
{
    public partial class TblProgram
    {
        public TblProgram()
        {
            
        }

        public int idProgram { get; set; }
        public string Name { get; set; }
        public DateTime validityDate { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace OpenImis.DB.SqlServer
{
    public partial class TblProgram_user
    {
        public int id { get; set; }
        public int program_id { get; set; }
        public int interactiveuser_id { get; set; }
    }
}

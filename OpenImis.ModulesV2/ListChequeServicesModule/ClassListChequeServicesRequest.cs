using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OpenImis.DB.SqlServer;
using System.Data.Common;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Xml;
using System.Diagnostics;

namespace OpenImis.ModulesV2.ListChequeServicesModule
{
    public class ClassListChequeServicesRequest
    {
        public ClassListChequeServicesRequest()
        {
        }
        public IEnumerable<List<Dictionary<string, object>>> ListChequeServ()
        {
            var results_list_all = new List<List<Dictionary<string, object>>>();


            return results_list_all;
        }
    }
    
}

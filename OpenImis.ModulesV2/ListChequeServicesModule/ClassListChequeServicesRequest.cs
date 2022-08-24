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
using OpenImis.ModulesV2.ServiceAllModule;
using OpenImis.ModulesV2.MainItemModule;
using OpenImis.ModulesV2.SubServiceModule;
using OpenImis.ModulesV2.SubItemModule;

namespace OpenImis.ModulesV2.ListChequeServicesModule
{
    public class ClassListChequeServicesRequest
    {
        public ClassListChequeServicesRequest()
        {
        }
        public List<IEnumerable<Dictionary<string, object>>> ListChequeServ()
        {
            var results_list_all = new List<IEnumerable<Dictionary<string, object>>>();

            ClassServiceAllRequest serviceAll = new ClassServiceAllRequest();
            ClassMainItemRequest mainItem = new ClassMainItemRequest();
            ClassSubServiceRequest subService = new ClassSubServiceRequest();
            ClassSubItemRequest subItem = new ClassSubItemRequest();
            var serv = serviceAll.SerializeDr(); var item = mainItem.SerializeDr(); var subserv = subService.SerializeDr(); var subIte = subItem.SerializeDr();

            results_list_all.Add(serv); results_list_all.Add(item); results_list_all.Add(subserv); results_list_all.Add(subIte);

            return results_list_all;
        }
    }
    
}

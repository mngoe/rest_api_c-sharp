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

namespace OpenImis.ModulesV2.ChequeModule
{
   public class ClassChequeRequest
    {

        public ClassChequeRequest()
        {
        }

        public IEnumerable<Dictionary<string, object>> SerializeDr()
        {
                var results = new List<Dictionary<string, object>>();
                try
                {
                    using (var imisContext = new ImisDB())
                    {
                        var sql = "SELECT chequeImportLineCode, chequeImportLineStatus FROM tblChequeSanteImportLine ORDER BY idChequeImportLine";
                        DbConnection connection = imisContext.Database.GetDbConnection();
                        using (DbCommand cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = sql;
                            if (connection.State.Equals(ConnectionState.Closed)) connection.Open();

                             using (var reader = cmd.ExecuteReader())
                                {
                                    var cols = new List<string>();
                                    for (var i = 0; i < reader.FieldCount; i++)
                                        cols.Add(reader.GetName(i));

                                    while (reader.Read())
                                        results.Add(SerializeRowDr(cols, reader));
                                    reader.Close();
                                }

                        }

                    
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }

            return results;
        }


        private Dictionary<string, object> SerializeRowDr(IEnumerable<string> cols,
                                                        DbDataReader reader)
        {
            var result = new Dictionary<string, object>();
            foreach (var col in cols)
                result.Add(col, reader[col]);
            return result;
        }

    }
}

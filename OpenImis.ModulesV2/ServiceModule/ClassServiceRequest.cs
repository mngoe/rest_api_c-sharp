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

namespace OpenImis.ModulesV2.ServiceModule
{
   public class ClassServiceRequest
    {

        public ClassServiceRequest()
        {
        }

        public IEnumerable<Dictionary<string, object>> SerializeDr()
        {

            StringBuilder s = new StringBuilder();
            var results = new List<Dictionary<string, object>>();
            var results_list_all = new List<Dictionary<string,  object>>();
            try
            {
                using (var imisContext = new ImisDB())
                {
                    var sql = "SELECT tblServices.ServiceID, ServiceUUID, ServCode, ServName, ServType,  ServLevel, ServPrice, ServCareType, ServFrequency, ServPatCat," +
                                      "tblServices.ValidityFrom, tblServices.ValidityTo, tblServices.LegacyID, tblServices.AuditUserID, tblServices.RowID, ServCategory, ServPackageType, manualPrice," +
                                      "tblProductContainedPackage.ItemID AS Items, tblServiceContainedPackage.ServiceId AS ServicePack " +
                                      "FROM tblServices" + " INNER JOIN tblServiceContainedPackage" +
                                      " ON tblServices.ServiceID = tblServiceContainedPackage.ServiceLinked" + " INNER JOIN tblProductContainedPackage" +
                                      " ON tblServiceContainedPackage.ServiceLinked = tblProductContainedPackage.ServiceID";

                    DbConnection connection = imisContext.Database.GetDbConnection();
                    using (DbCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        if (connection.State.Equals(ConnectionState.Closed)) connection.Open();

                        using (var reader = cmd.ExecuteReader())
                        {
                            var cols = new List<string>(); // create a list of string
                            for (var i = 0; i < reader.FieldCount; i++)
                                cols.Add(reader.GetName(i));

                            var temp = "";
                            var getId = "";
                            while (reader.Read())
                            {
                                getId = Convert.ToString(reader["ServiceID"]); //convert the ID got into string
                                if (temp != getId)
                                {
                                    results_list_all.Add(SerializeRow_SubRow_Dr(cols, reader, getId)); // add in the list, the results of serialized data from executed request
                                    temp = getId;
                                }

                            }
                            reader.Close();

                        }
                    }

                   
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
     
                return results_list_all;
        }


        private Dictionary<string,  object> SerializeRow_SubRow_Dr(IEnumerable<string> cols,
                                                       DbDataReader rder, string id)
        {
            var results = new Dictionary<string, object>();
            var result_lis_sub_req_serv = new List<Dictionary<string, object>>();
            var result_lis_sub_req_item = new List<Dictionary<string, object>>();

            foreach (var col in cols)
                    {
                        if (col == "ServicePack")
                        {
                            try
                            {
                                using (var imisContext = new ImisDB())
                                {
                                    var sql = "SELECT * From tblServiceContainedPackage where tblServiceContainedPackage.ServiceLinked =" + id;
                                    DbConnection connection = imisContext.Database.GetDbConnection();
                                    using (DbCommand cmd = connection.CreateCommand())
                                    {
                                        cmd.CommandText = sql;
                                        if (connection.State.Equals(ConnectionState.Closed)) connection.Open();

                                        using (var reader = cmd.ExecuteReader())
                                        {
                                            var cols_sub = new List<string>();
                                            for (var i = 0; i < reader.FieldCount; i++)
                                                cols_sub.Add(reader.GetName(i)); // insert field column within the list of string
                                            while (reader.Read())
                                                result_lis_sub_req_serv.Add(SerializeRowDr(cols_sub, reader));

                                            results.Add(col, result_lis_sub_req_serv); // add within the Dictionary the serialize data where key is the list of field and the value is a dictionary
                                            reader.Close();
                                        }

                                    }
                                }
                               
                            }
                            catch (Exception ex)
                            {
                                Console.Write(ex.Message);
                            }

                        }
                            
                        else if(col == "Items") // In the column is ItemID
                        {
                            try
                            {
                                using (var imisContext = new ImisDB())
                                {
                                    var sql = "SELECT * From tblProductContainedPackage where tblProductContainedPackage.ServiceId =" + id;
                                    DbConnection connection = imisContext.Database.GetDbConnection();
                                    using (DbCommand cmd = connection.CreateCommand())
                                    {
                                        cmd.CommandText = sql;
                                        if (connection.State.Equals(ConnectionState.Closed)) connection.Open();

                                        using (var reader = cmd.ExecuteReader())
                                        {
                                            var cols_sub = new List<string>();
                                            for (var i = 0; i < reader.FieldCount; i++)
                                                cols_sub.Add(reader.GetName(i)); // insert field column within the list of string
                                            while (reader.Read())
                                            result_lis_sub_req_item.Add(SerializeRowDr(cols_sub, reader));

                                            results.Add(col, result_lis_sub_req_item); // add within the Dictionary the serialize data where key is the list of field and the value is a dictionary
                                            reader.Close();
                                        }

                                    }
                                }
                           
                            }
                            catch (Exception ex)
                            {
                                Console.Write(ex.Message);
                            }
                        }
                       
                        else
                            results.Add(col, rder[col]);      
            }
            
            return results;
        }

        private Dictionary<string, object> SerializeRowDr(IEnumerable<string> cols,
                                                        DbDataReader dr) // function to serialize sub data requested
        {
            var result = new Dictionary<string, object>();
            foreach (var col in cols)
                result.Add(col, dr[col]);
            return result;
        }

    }
}

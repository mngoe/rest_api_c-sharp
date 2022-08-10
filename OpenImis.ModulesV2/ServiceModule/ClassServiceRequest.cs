using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Text;

namespace OpenImis.ModulesV2.ServiceModule
{
   public class ClassServiceRequest
    {

        public ClassServiceRequest()
        {
        }

        SqlConnection conn = new SqlConnection("Data Source = localhost;" + "Initial Catalog=openimisproductDevDbServer;" + "User ID=openimis;" + "Password=openimis22;" +
            "integrated security = SSPI;" + "MultipleActiveResultSets =True;");
        SqlDataReader dr = null;

        SqlConnection conn_sub = new SqlConnection("Data Source = localhost;" + "Initial Catalog=openimisproductDevDbServer;" + "User ID=openimis;" + "Password=openimis22;" +
            "integrated security = SSPI;" + "MultipleActiveResultSets=True;"); // create a new connection in same time to avoid or fix problem multiple results when a sqldatareader is used or opened
        SqlDataReader dr_sub = null;

        public IEnumerable<Dictionary<string, object>> SerializeDr()
        {

            SqlCommand cmd = new SqlCommand(); //create an instance of Sql command object

            
            StringBuilder s = new StringBuilder();
            var results = new List<Dictionary<string, object>>();
            var results_list_all = new List<Dictionary<string,  object>>();
            try
            {

                cmd.CommandTimeout = 60; //specify the time (second)
                cmd.Connection = conn; // copy connection string
                cmd.CommandType = CommandType.Text;

                
                

                //open the connection
                conn.Open();
                if (conn.State == ConnectionState.Open)// check the state of connection
                {
                    Console.WriteLine("Connection was succesfull \n");
                    cmd.CommandText = "SELECT * From [openimisproductDevDbServer].[dbo].[tblServiceProductItems]";

                    //get the query result
                    dr = cmd.ExecuteReader(CommandBehavior.SingleResult);

                    var cols = new List<string>(); // create a list of string
                    for (var i = 0; i < dr.FieldCount; i++)
                        cols.Add(dr.GetName(i));

                    var temp = "";
                    var getId = "";
                    while (dr.Read())
                    {
                        getId = Convert.ToString(dr["ServiceID"]); //convert the ID got into string
                        if (temp != getId)
                        {
                            results_list_all.Add(SerializeRow_SubRow_Dr(cols, dr, getId)); // add in the list, the results of serialized data from executed request
                            temp = getId;
                        }
                     
                    }
                    dr.Close();

                }
            }
                catch (Exception ex) 
            {
                Console.Write(ex.Message);
            }
                finally
            {
                conn.Close(); //close the connection
            }

            return results_list_all;
        }


        private Dictionary<string,  object> SerializeRow_SubRow_Dr(IEnumerable<string> cols,
                                                       SqlDataReader dr, string id)
        {
            var results = new Dictionary<string, object>();
            var result_lis_sub_req_serv = new List<Dictionary<string, object>>();
            var result_lis_sub_req_item = new List<Dictionary<string, object>>();
            SqlCommand cmd_sub = new SqlCommand();
            cmd_sub.CommandTimeout = 60; //specify the time (second)
            cmd_sub.Connection = conn_sub; // copy connection string
            cmd_sub.CommandType = CommandType.Text;

            foreach (var col in cols)
                    {
                        if (col == "ServicePack")
                        {
                            try
                            {

                                conn_sub.Open();
                                if (conn_sub.State == ConnectionState.Open) // check the state of connection
                                {
                            cmd_sub.CommandText = "SELECT * From [openimisproductDevDbServer].[dbo].[tblServiceContainedPackage] where tblServiceContainedPackage.ServiceLinked =" + id;
                                    //get the query result
                                    dr_sub = cmd_sub.ExecuteReader(CommandBehavior.SingleResult);
                                    var cols_sub = new List<string>();
                                    for (var i = 0; i < dr_sub.FieldCount; i++)
                                        cols_sub.Add(dr_sub.GetName(i)); // insert field column within the list of string
                                    while (dr_sub.Read())
                                        result_lis_sub_req_serv.Add(SerializeRowDr(cols_sub, dr_sub));

                                    results.Add(col, result_lis_sub_req_serv); // add within the Dictionary the serialize data where key is the list of field and the value is a dictionary.


                                    dr_sub.Close();
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.Write(ex.Message);
                            }
                            finally
                            {
                                conn_sub.Close(); //close the connection

                            }

                        }
                            
                        else if(col == "Items") // In the column is ItemID
                        {
                            try
                            {
                                conn_sub.Open();
                                if (conn_sub.State == ConnectionState.Open)
                                {
                            cmd_sub.CommandText = "SELECT * From [openimisproductDevDbServer].[dbo].[tblProductContainedPackage] where tblProductContainedPackage.ServiceId =" + id;
                                    //get the query result
                                    dr_sub = cmd_sub.ExecuteReader(CommandBehavior.SingleResult);
                                    var cols_sub = new List<string>();
                                    for (var i = 0; i < dr_sub.FieldCount; i++)
                                        cols_sub.Add(dr_sub.GetName(i));
                                    while (dr_sub.Read())
                                        result_lis_sub_req_item.Add(SerializeRowDr(cols_sub, dr_sub));

                                    results.Add(col, result_lis_sub_req_item); // add within the Dictionary the serialize data where key is the list of field and the value is a dictionary.
                                    dr_sub.Close();
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.Write(ex.Message);
                            }
                            finally
                            {
                                conn_sub.Close(); //close the connection

                            }
                        }
                       
                        else
                            results.Add(col, dr[col]);      
                    }
            
            return results;
        }

        private Dictionary<string, object> SerializeRowDr(IEnumerable<string> cols,
                                                        SqlDataReader dr) // function to serialize sub data requested
        {
            var result = new Dictionary<string, object>();
            foreach (var col in cols)
                result.Add(col, dr[col]);
            return result;
        }

    }
}

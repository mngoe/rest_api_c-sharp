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

        SqlConnection conn = new SqlConnection("Data Source = localhost;" + "Initial Catalog=openimisproductNewdb;" + "User ID=openimis;" + "Password=openimis22;" +
            "integrated security = SSPI;" + "MultipleActiveResultSets =True;");
        SqlDataReader dr = null;

        SqlConnection conn_sub = new SqlConnection("Data Source = localhost;" + "Initial Catalog=openimisproductNewdb;" + "User ID=openimis;" + "Password=openimis22;" +
            "integrated security = SSPI;" + "MultipleActiveResultSets=True;");
        SqlDataReader dr_sub = null;

        SqlConnection conn_sub2 = new SqlConnection("Data Source = localhost;" + "Initial Catalog=openimisproductNewdb;" + "User ID=openimis;" + "Password=openimis22;" +
            "integrated security = SSPI;" + "MultipleActiveResultSets=True;");
        SqlDataReader dr_sub2 = null;



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
                    cmd.CommandText = "SELECT * From [openimisproductNewdb].[dbo].[tblServiceProductItems]";

                    //get the query result
                    dr = cmd.ExecuteReader(CommandBehavior.SingleResult);

                    var cols = new List<string>();
                    for (var i = 0; i < dr.FieldCount; i++)
                        cols.Add(dr.GetName(i));


                    while (dr.Read())
                    {
                        var getId = Convert.ToString(dr["ServiceID"]);
                        results_list_all.Add(SerializeRow_SubRow_Dr(cols, dr, getId));
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
            var result_sub_req = new Dictionary<string, object>();
            SqlCommand cmd_sub = new SqlCommand();
            SqlCommand cmd_sub2 = new SqlCommand();


                    foreach (var col in cols)
                    {
                        if (col == "ServiceIDFkSCP")
                        {
                            try
                            {
                                cmd_sub.CommandTimeout = 60; //specify the time (second)
                                cmd_sub.Connection = conn_sub; // copy connection string
                                cmd_sub.CommandType = CommandType.Text;

                                conn_sub.Open();
                                if (conn_sub.State == ConnectionState.Open) // check the state of connection
                                {
                                    cmd_sub.CommandText = "SELECT * From [openimisproductNewdb].[dbo].[tblServiceContainedPackage] where tblServiceContainedPackage.ServiceId =" + id;
                                    //get the query result
                                    dr_sub = cmd_sub.ExecuteReader(CommandBehavior.SingleRow);
                                    var cols_sub = new List<string>();
                                    for (var i = 0; i < dr_sub.FieldCount; i++)
                                        cols_sub.Add(dr_sub.GetName(i));
                                    while (dr_sub.Read())
                                        results.Add(col, SerializeRowDr(cols_sub, dr_sub));
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
                            
                        else if(col == "ItemID")
                        {
                            try
                            {
                                cmd_sub2.CommandTimeout = 60; //specify the time (second)
                                cmd_sub2.Connection = conn_sub2; // copy connection string
                                cmd_sub2.CommandType = CommandType.Text;

                                conn_sub2.Open();
                                if (conn_sub2.State == ConnectionState.Open)
                                {
                                    cmd_sub2.CommandText = "SELECT * From [openimisproductNewdb].[dbo].[tblProductContainedPackage] where tblProductContainedPackage.ServiceId =" + id;
                                    //get the query result
                                    dr_sub2 = cmd_sub2.ExecuteReader(CommandBehavior.SingleRow);
                                    var cols_sub2 = new List<string>();
                                    for (var i = 0; i < dr_sub2.FieldCount; i++)
                                        cols_sub2.Add(dr_sub2.GetName(i));
                                    while (dr_sub2.Read())
                                        results.Add(col, SerializeRowDr(cols_sub2, dr_sub2));
                                    dr_sub2.Close();
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.Write(ex.Message);
                            }
                            finally
                            {
                                conn_sub2.Close(); //close the connection

                            }
                        }
                       
                        else
                            results.Add(col, dr[col]);      
                    }
            
            return results;
        }

        private Dictionary<string, object> SerializeRowDr(IEnumerable<string> cols,
                                                        SqlDataReader dr)
        {
            var result = new Dictionary<string, object>();
            foreach (var col in cols)
                result.Add(col, dr[col]);
            return result;
        }

        private Dictionary<string, object> SerializeRowDr2(string Ids,
                                                        SqlDataReader dr)
        {


            var result = new Dictionary<string, object>();
            //foreach (var col in cols)
              //  result.Add(col, dr[col]);
            return result;
        }

    }
}

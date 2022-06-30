using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Text;

namespace OpenImis.ModulesV2.ChequeModule
{
   public class ClassChequeRequest
    {

        public ClassChequeRequest()
        {
        }

        SqlConnection conn = new SqlConnection("Data Source = localhost;" + "Initial Catalog=openimisproductNewdb;" + "User ID=openimis;" + "Password=openimis22;");
        SqlDataReader dr = null;

        /*public String GetdataFromSQLDB()
        {
            SqlCommand cmd = new SqlCommand(); //create an instance of Sql command object
            StringBuilder s = new StringBuilder();
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
                    cmd.CommandText = "SELECT * FROM [openimisproductNewdb].[dbo].[tblChequeSanteImportLine] ORDER BY [idChequeImportLine]";

                    //get the query result
                    dr = cmd.ExecuteReader(CommandBehavior.SingleResult);

                    s.Append("{");
                    s.Append("[");
                    if (dr.HasRows)

                        while (dr.Read())

                            s.Append("{" + "ID Cheque Import Line" + ":" + dr["idChequeImportLine"] + ", "
                            + "cheque ImportId Id" + ":" + dr["chequeImportId_id"] + ", "
                            + "cheque Import Line Code" + ":" + dr["chequeImportLineCode"] + ","
                            + "cheque Import Line Date" + ":" + dr["chequeImportLineDate"] + ","
                            + "chequeImportLineStatus" + ":" + dr["chequeImportLineStatus"] + "}, ");
                    s.Remove(s.Length - 2, 2);
                    s.Append("]");
                    s.Append("}");
 
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
            return s.ToString();
        }*/


        public IEnumerable<Dictionary<string, object>> SerializeDr()
        {

            SqlCommand cmd = new SqlCommand(); //create an instance of Sql command object
            StringBuilder s = new StringBuilder();
            var results = new List<Dictionary<string, object>>();
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
                    cmd.CommandText = "SELECT * FROM [openimisproductNewdb].[dbo].[tblChequeSanteImportLine] ORDER BY [idChequeImportLine]";

                    //get the query result
                    dr = cmd.ExecuteReader(CommandBehavior.SingleResult);

                    var cols = new List<string>();
                    for (var i = 0; i < dr.FieldCount; i++)
                        cols.Add(dr.GetName(i));

                    while (dr.Read())
                        results.Add(SerializeRowDr(cols, dr));
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

    }
}

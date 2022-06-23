﻿using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace OpenimisStmt_Cheque
{
    public class ClassChequeRequest
    {
        public ClassChequeRequest()
        {
        }

        SqlConnection conn = new SqlConnection("Data Source = localhost;" + "Initial Catalog=openimisproductNewdb;" + "User ID=openimis;" + "Password=openimis22;");
        SqlDataReader dr = null;  

        public ConnectionToDB()
        {
            try
            {
                SqlCommand cmd = new SqlCommand(); //create an instance of Sql command object
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
                    Console.WriteLine("Succesfully Selected \n");
                    // For loop to read everything from the table 

                    while (dr.Read()) // when row exists Read row from the table  
                        Console.WriteLine("ID Cheque importLine: {0} \t Cheque ImportID_Id: {1} \t  Cheque ImportLine Code:{2} \t  Cheque ImportLine Date:{3}" +
                            " \t  Cheque ImportLine Status: {4}", dr[0], dr[1], dr[2], dr[3], dr[4]);
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
            Console.WriteLine("\n\n Press any key to quite");
            Console.Read();
        }
    }
        
}


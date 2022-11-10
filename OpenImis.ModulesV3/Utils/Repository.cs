﻿using OpenImis.DB.SqlServer;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace OpenImis.ModulesV3.Utils
{
    public class Repository
    {
        public string GetLoginNameByUserUUID(Guid userUUID)
        {
            string response;

            try
            {
                using (var imisContext = new ImisDB())
                {
                    response = imisContext.TblUsers
                        .Where(u => u.UserUUID == userUUID)
                        .Select(x => x.LoginName)
                        .FirstOrDefault();
                }

                return response;
            }
            catch (SqlException e)
            {
                throw e;
            }
        }
    }
}

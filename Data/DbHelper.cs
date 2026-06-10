using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;

namespace BreweryApp.Data
{
    public static class DbHelper
    {
        private static readonly string connectionString =
            @"Server=ANGELOCHEK;Database=Kurs.Beer.Hramov;Trusted_Connection=True;TrustServerCertificate=True";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}

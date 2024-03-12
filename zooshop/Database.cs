using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace zooshop
{
    internal class Database
    {
        NpgsqlConnection conn = new NpgsqlConnection("Server = localhost; Port = 5432; Database = zooshop; User Id = postgres; Password = assaq123;");

        public void openConnection ()
        {
            if (conn.State == System.Data.ConnectionState.Closed)
            {
                conn.Open();
            }
        }
        public void closeConnection()
        {
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
        }
        public NpgsqlConnection getConnection ()
        {
            return conn;
        }
    }
}

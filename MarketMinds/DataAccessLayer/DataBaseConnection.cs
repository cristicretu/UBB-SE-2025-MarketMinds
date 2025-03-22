using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace DataAccessLayer
{
    public class DataBaseConnection
    {
        SqlConnection sqlConnection;

        private string connectionString = "Server=COSTIN\\SQLEXPRESS;Database=MarketPlace;User Id=sa;Password=Spiderman2004;TrustServerCertificate=True";



        public DataBaseConnection()
        {
            this.sqlConnection = new SqlConnection(connectionString);
        }

        public SqlConnection GetConnection()
        {
            return this.sqlConnection;
        }

        public void OpenConnection()
        {
            if (this.sqlConnection.State != System.Data.ConnectionState.Open)
            {
                this.sqlConnection.Open();
            }
        }

        public void CloseConnection()
        {
            if (this.sqlConnection.State == System.Data.ConnectionState.Open)
            {
                this.sqlConnection.Close();
            }
        }

    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataAccessLayer
{
    public class DataBaseConnection
    {
        private SqlConnection sqlConnection;
        private readonly string connectionString;
        private readonly IConfiguration configuration;

        public DataBaseConnection(IConfiguration config)
        {
            configuration = config;
            string? localDataSource = configuration["LocalDataSource"];
            string? initialCatalog = configuration["InitialCatalog"];

            connectionString = "Data Source=" + initialCatalog + ";" +
                       "Initial Catalog=" + localDataSource + ";" +
                       "Integrated Security=True;" +
                       "TrustServerCertificate=True";
            try
            {
                sqlConnection = new SqlConnection(connectionString);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error initializing SQL connection: {ex.Message}");
            }
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

        //  public T? ExecuteScalar<T>(string storedProcedure, SqlParameter[]? sqlParameters = null)
        // {
        //     try
        //     {
        //         OpenConnection();
        //         using (SqlCommand command = new SqlCommand(storedProcedure, sqlConnection))
        //         {
        //             command.CommandType = CommandType.StoredProcedure;

        //             if (sqlParameters != null)
        //             {
        //                 command.Parameters.AddRange(sqlParameters);
        //             }

        //             var result = command.ExecuteScalar();
        //             if (result == DBNull.Value || result == null)
        //             {
        //                 return default;
        //             }

        //             return (T)Convert.ChangeType(result, typeof(T));
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         throw new Exception($"Error - ExecutingScalar: {ex.Message}");
        //     }
        //     finally
        //     {
        //         CloseConnection();
        //     }
        // }

        // public DataTable ExecuteReader(string storedProcedure, SqlParameter[]? sqlParameters = null)
        // {
        //     try
        //     {
        //         OpenConnection();
        //         using (SqlCommand command = new SqlCommand(storedProcedure, sqlConnection))
        //         {
        //             command.CommandType = CommandType.StoredProcedure;

        //             if (sqlParameters != null)
        //             {
        //                 command.Parameters.AddRange(sqlParameters);
        //             }

        //             using (SqlDataReader reader = command.ExecuteReader())
        //             {
        //                 DataTable dataTable = new DataTable();
        //                 dataTable.Load(reader);
        //                 return dataTable;
        //             }
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         throw new Exception($"Error - ExecuteReader: {ex.Message}");
        //     }
        //     finally
        //     {
        //         CloseConnection();
        //     }
        // }

        // public int ExecuteNonQuery(string storedProcedure, SqlParameter[]? sqlParameters = null)
        // {
        //     try 
        //     {
        //         OpenConnection();
        //         using (SqlCommand command = new SqlCommand(storedProcedure, sqlConnection))
        //         {
        //             command.CommandType = CommandType.StoredProcedure;

        //             if (sqlParameters != null)
        //             {
        //                 command.Parameters.AddRange(sqlParameters);
        //             }

        //             return command.ExecuteNonQuery();
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         throw new Exception($"Error - ExecuteNonQuery: {ex.Message}");
        //     }
        //     finally
        //     {
        //         CloseConnection();
        //     }
        // }

    }
}
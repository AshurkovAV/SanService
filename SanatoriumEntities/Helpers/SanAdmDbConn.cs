using SanatoriumEntities.Exceptions;
using System;
using System.Data.SqlClient;

namespace SanatoriumEntities.Helpers
{
    public class SanAdmDbConn
    {
        public const string DB_NAME = "san_administration";

        public static int sqlQuery(string queryString)
        {
            using (SqlConnection connection = new SqlConnection(SanAdmDbConn.getConnectionString()))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Connection.Open();

                return command.ExecuteNonQuery();
            }
        }

        public static int sqlQueryInTransaction(string queryString, int resultExpectedRows = -1)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(SanAdmDbConn.getConnectionString()))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction;

                transaction = connection.BeginTransaction($"Transaction[0]");

                command.Connection = connection;
                command.Transaction = transaction;
                try
                {
                    command.CommandText = queryString;
                    result = command.ExecuteNonQuery();

                    if (resultExpectedRows >= 0)
                    {
                        if (resultExpectedRows == (int)(decimal)result)
                        {
                            transaction.Commit();
                        }
                        else
                        {
                            throw new IvalidExpectedDataException($"Expected: [{resultExpectedRows.ToString("000")}], actual: [{result.ToString("000")}]");
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                    Console.WriteLine("  Message: {0}", ex.Message);

                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                        Console.WriteLine("  Message: {0}", ex2.Message);
                    }
                }
            }

            return (int)(decimal)result;
        }

        public static object sqlScalarQuery(string queryString)
        {
            using (SqlConnection connection = new SqlConnection(SanAdmDbConn.getConnectionString()))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Connection.Open();

                return command.ExecuteScalar();
            }
        }

        public static string getConnectionString()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            builder.UserID = "sa";
            builder.Password = "Hospital6";
            builder.DataSource = @"192.168.10.2\sqlexpress";
            builder.InitialCatalog = DB_NAME;
            builder.MinPoolSize = 9;

            return builder.ConnectionString;
        }
    }
}
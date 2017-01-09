using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplicationWriterPrice
{
    class Program
    {
        private static readonly DataTable DataTable = new DataTable();

        static void Main(string[] args)
        {
            FillDataTable();

            var numberOfItems = DataTable.Rows.Count;
            Random stockIndex = new Random();
            Random stockPrice = new Random();

            Console.WriteLine("Press ESC to stop");
            do
            {
                while (!Console.KeyAvailable)
                {
                    var index = stockIndex.Next(1, numberOfItems);
                    var stockCode = DataTable.Rows[index - 1]["Code"].ToString();

                    var connString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                    var query = $"update Stocks set Price = @price where Code = '{stockCode}'";

                    var conn = new SqlConnection(connString);
                    var cmd = new SqlCommand(query, conn);

                    var newPrice = RandomDouble(stockPrice, 1, 100);
                    cmd.Parameters.AddWithValue("@price", newPrice);
                    conn.Open();

                    Console.WriteLine($"Writing {stockCode} = {newPrice:N2}");
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    conn.Close();

                    Thread.Sleep(350);
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }

        private static double RandomDouble(Random rand, double start, double end)
        {
            return (rand.NextDouble() * Math.Abs(end - start)) + start;
        }

        private static void FillDataTable()
        {
            var connString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            var query = "select * from Stocks";

            var conn = new SqlConnection(connString);
            var cmd = new SqlCommand(query, conn);
            conn.Open();

            var da = new SqlDataAdapter(cmd);
            da.Fill(DataTable);
            conn.Close();
            da.Dispose();
        }
    }
}

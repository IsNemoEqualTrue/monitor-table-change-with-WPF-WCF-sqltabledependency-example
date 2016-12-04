using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.ServiceModel;
using ConsoleApplicationServer.Models;
using ConsoleApplicationServer.PriceChangeContracts;
using TableDependency.EventArgs;
using TableDependency.SqlClient;

namespace ConsoleApplicationServer.PriceChangeService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    public class PriceTicker : IPriceTicker, IDisposable
    {
        #region Instance variables

        private readonly List<IPriceChangeCallBack> _callbackList = new List<IPriceChangeCallBack>();
        private readonly string _connectionString;
        private readonly SqlTableDependency<Stock> _sqlTableDependency;

        #endregion

        #region Constructors

        public PriceTicker()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            _sqlTableDependency = new SqlTableDependency<Stock>(_connectionString, "Stocks");

            _sqlTableDependency.OnChanged += TableDependency_Changed;
            _sqlTableDependency.OnError += (sender, args) => Console.WriteLine($"Error: {args.Message}");
            _sqlTableDependency.Start();

            Console.WriteLine(@"Waiting for receiving notifications...");
        }

        #endregion

        #region SqlTableDependency

        private void TableDependency_Changed(object sender, RecordChangedEventArgs<Stock> e)
        {
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine($"DML: {e.ChangeType}");
            Console.WriteLine($"Code: {e.Entity.Code}");
            Console.WriteLine($"Name: {e.Entity.Name}");
            Console.WriteLine($"Price: {e.Entity.Price}");

            this.PublishPriceChange(e.Entity.Code, e.Entity.Name, e.Entity.Price);
        }

        #endregion

        #region Publish-Subscribe design pattern

        public IList<Stock> GetAllStocks()
        {
            var stocks = new List<Stock>();

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "SELECT * FROM [Stocks]";

                    using (var sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        while (sqlDataReader.Read())
                        {
                            var code = sqlDataReader.GetString(sqlDataReader.GetOrdinal("Code"));
                            var name = sqlDataReader.GetString(sqlDataReader.GetOrdinal("Name"));
                            var price = sqlDataReader.GetDecimal(sqlDataReader.GetOrdinal("Price"));

                            stocks.Add(new Stock { Code = code, Name = name, Price = price });
                        }
                    }
                }
            }

            return stocks;
        }

        public void Subscribe()
        {
            var registeredUser = OperationContext.Current.GetCallbackChannel<IPriceChangeCallBack>();
            if (!_callbackList.Contains(registeredUser))
            {
                _callbackList.Add(registeredUser);
            }
        }

        public void Unsubscribe()
        {
            var registeredUser = OperationContext.Current.GetCallbackChannel<IPriceChangeCallBack>();
            if (_callbackList.Contains(registeredUser))
            {
                _callbackList.Remove(registeredUser);
            }
        }

        public void PublishPriceChange(string code, string name, decimal price)
        {
            _callbackList.ForEach(delegate (IPriceChangeCallBack callback) { callback.PriceChange(code, name, price); });
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            _sqlTableDependency.Stop();
        }

        #endregion
    }
}
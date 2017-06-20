# Audit table change with WPF, WCF and SqlTableDependency

[SqlTableDependency](https://github.com/christiandelbianco/monitor-table-change-with-sqltabledependency) is an open source component that can create a series of database objects used to receive notifications on table record change. When any insert/update/delete operation is detected, a change notification containing the record’s status is sent to SqlTableDependency, thereby eliminating the need of an additional SELECT to update application’s data.

To get notifications, SqlTableDependency provides an on the fly low-level implementation of an infrastructure composed of a table trigger, contracts, messages, queue, service broker and a clean-up stored procedure.

SqlTableDependency class provides access to notifications without knowing anything about the underlying database infrastructure. When a record change happens, this infrastructure notifies SqlTableDependency, which in turn raises a .NET event to subscribers providing the updated record values.
Listen for table change alert
Using the SqlTableDependency is a good way to make your data driven application (whether it be Web or Windows Forms) more efficient by removing the need to constantly re-query your database checking for data changes.

**Instead of executing a request from client to the database, we do the reverse: sending a notification from database to clients applications**.

The following video show how to build a web application able to send real time notifications to clients. The code is visible below:

[![IMAGE ALT TEXT HERE](http://img.youtube.com/vi/c2LfyCmy65A/0.jpg)](https://www.youtube.com/watch?v=c2LfyCmy65A)

## Get notifications on record change using WPF and WCF
This example show how to keep up to date WPF client applications displaying Stock prices. Every WPF client has a grid that needs to be automatically updated whenever a stock price change.

### WCF server application implementing Publish-Subscribe pattern
Let's assume that we have a table as:

```SQL
CREATE TABLE [Stocks] (
	[Code] [nvarchar](50) NULL,
	[Name] [nvarchar](50) NULL,
	[Price] [decimal](18, 0) NULL)
```

that is continuously update with stock's value from an external thread. We want our WPF application be notified every time a new value is updated without polling periodically the Stocks table. This means we want receive notifications from our database on every table change.

To achieve this, we need a service application that will take care of create a SqlTableDependency object and for every change notification received, forward this new stock price to all interested WPF client applications.

For this we are going to use a WCF service implementing the Publish-Subscribe pattern. This service will act as stock price broker receiving database notifications on any stock price change and in turn will notify subscribed WCF client applications:

![alt text][shema]

[shema]: https://github.com/christiandelbianco/Monitor-table-change-with-WPF-WCF-sqltabledependency/blob/master/Schema-min.png "Notifications"

For simplicity, we create a console application acting as WCF host. In this application we create a C# model that will be filled by notification with the new table value:

```C#
public class Stock
{
    public decimal Price { get; set; }
    public string Symbol { get; set; }
    public string Name { get; set; }
}
```

After that, we create the WCF service interfaces:

```C#
    public interface IPriceChangeCallBack
    {
        [OperationContract]
        void PriceChange(string code, string name, decimal price);
    }


    [ServiceContract(CallbackContract = typeof(IPriceChangeCallBack))]
    public interface IPriceTicker
    {
        [OperationContract]
        void Subscribe();

        [OperationContract]
        void Unsubscribe();

        [OperationContract]
        IList<Stock> GetAllStocks();

        [OperationContract]
        void PublishPriceChange(string item, string name, decimal price);
    }
```

Now we install SqlTableDependency nuget package:

**PM> Install-Package SqlTableDependency**

```C#
We implement now the WCF service:

    [ServiceBehavior(
            InstanceContextMode = InstanceContextMode.Single, 
            ConcurrencyMode = ConcurrencyMode.Single)]
    public class PriceTicker : IPriceTicker, IDisposable
    {
        #region Instance variables

        private readonly List<IPriceChangeCallBack> _callbackList = 
                new List<IPriceChangeCallBack>();
        private readonly string _connectionString;
        private readonly SqlTableDependency<Stock> _sqlTableDependency;

        #endregion

        #region Constructors

        public PriceTicker()
        {
            _connectionString = ConfigurationManager
                        .ConnectionStrings["connectionString"]
                        .ConnectionString;

            _sqlTableDependency = new SqlTableDependency<Stock>(
                        _connectionString, 
                        "Stocks");

            _sqlTableDependency.OnChanged += TableDependency_Changed;
            _sqlTableDependency.OnError += (sender, args) => 
                        Console.WriteLine($"Error: {args.Message}");
            _sqlTableDependency.Start();

            Console.WriteLine(@"Waiting for receiving notifications...");
        }

        #endregion

        #region SqlTableDependency

        private void TableDependency_Changed(
            object sender, 
            RecordChangedEventArgs<Stock> e)
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
                            var code = sqlDataReader
                                    .GetString(sqlDataReader
                                    .GetOrdinal("Code"));
                            var name = sqlDataReader
                                    .GetString(sqlDataReader
                                    .GetOrdinal("Name"));
                            var price = sqlDataReader
                                    .GetDecimal(sqlDataReader
                                    .GetOrdinal("Price"));

                            stocks.Add(new Stock { 
                                    Code = code, 
                                    Name = name, 
                                    Price = price });
                        }
                    }
                }
            }

            return stocks;
        }

        public void Subscribe()
        {
            var registeredUser = OperationContext.
                        Current
                        .GetCallbackChannel<IPriceChangeCallBack>();
            if (!_callbackList.Contains(registeredUser))
            {
                _callbackList.Add(registeredUser);
            }
        }

        public void Unsubscribe()
        {
            var registeredUser = OperationContext
                        .Current
                        .GetCallbackChannel<IPriceChangeCallBack>();
            if (_callbackList.Contains(registeredUser))
            {
                _callbackList.Remove(registeredUser);
            }
        }

        public void PublishPriceChange(string code, string name, decimal price)
        {
            _callbackList.ForEach(delegate (IPriceChangeCallBack callback) { 
                        callback.PriceChange(code, name, price); 
            });
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            _sqlTableDependency.Stop();
        }

        #endregion
    }
}}
```

We set the endpoint binding as:

```XML
<?xml version="1.0" encoding="utf-8"?>
<configuration>

  <connectionStrings>
    <add name="connectionString" connectionString="data source=.;initial catalog=TableDependencyDB;integrated security=False; User ID=Test_User;Password=Casadolcecasa1" providerName="System.Data.SqlClient"/>
  </connectionStrings>

  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.6"/>
  </startup>

  <system.serviceModel>
    <behaviors>
      <serviceBehaviors>
        <behavior>
          <serviceMetadata httpGetEnabled="true"/>
          <serviceDebug includeExceptionDetailInFaults="true"/>
        </behavior>
      </serviceBehaviors>
    </behaviors>

    <services>
      <service name="ConsoleApplicationServer.PriceChangeService.PriceTicker">
        <endpoint address="get" binding="wsDualHttpBinding" contract="ConsoleApplicationServer.PriceChangeContracts.IPriceTicker">
          <identity>
            <dns value="localhost" />
          </identity>
        </endpoint>
        <endpoint address="mex" binding="mexHttpBinding" contract="IMetadataExchange" />
        <host>
          <baseAddresses>
            <add baseAddress="http://localhost:8090/PriceTickerService/" />
          </baseAddresses>
        </host>
      </service>
    </services>
  </system.serviceModel>

</configuration>
```

And to conclude, we code the hosting part:

```C#
    class Program
    {
        static void Main(string[] args)
        {
            var host = new ServiceHost(typeof(PriceTicker));
            host.Open();
            Console.WriteLine($"Service started at {host.Description.Endpoints[0].Address}");
            Console.WriteLine("Press key to stop the service.");
            Console.ReadLine();
            host.Close();
        }
    }
```

### WPF client applications
As first step we need to create a WCF's proxy to execute the price change subscription and of course to read the initial list of stocks. This operation can be done from visual studio: run the server application and then Add a service reference typing the WCF url end point:

![alt text][proxy]

[proxy]: https://github.com/christiandelbianco/Monitor-table-change-with-WPF-WCF-sqltabledependency/blob/master/ProxyGeneration-min.png "Proxy"

We prepare the layout as follow:

```XML
<Window x:Class="DataGridSample.Window1"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="DataGrid Sample" Height="350" Width="776">
    <Grid>
        <DataGrid Height="302" Width="750" HorizontalAlignment="Left" Margin="10,10,0,0" 
          Name="McDataGrid" VerticalAlignment="Top" RowHeight="30" ColumnWidth="auto" 
                  ColumnHeaderHeight="30" HeadersVisibility="Column" AlternatingRowBackground="Silver"
                  BorderBrush="Gray" BorderThickness="1" AutoGenerateColumns="False">

            <DataGrid.Columns>
                <DataGridTextColumn Header="Code" Binding="{Binding Code}" />
                <DataGridTextColumn Header="Name" Binding="{Binding Name}" />
                <DataGridTextColumn Header="Price" Binding="{Binding Price}" />
            </DataGrid.Columns>

        </DataGrid>
    </Grid>
</Window>
```

Our client application executes an initial request to WCF, just to fill its grid. After that, the application subscribe its self as listener for price change notifications. In this way, every time a stock is updated, a notification containing fresh values is received:

```C#
    public partial class Window1 : Window, IPriceTickerCallback
    {
        private readonly IList<Stock> _stocks;
        private readonly PriceTickerClient _proxy;

        public Window1()
        {
            this.InitializeComponent();
           
            var instanceContext = new InstanceContext(this);
            _proxy = new PriceTickerClient(instanceContext);
            _proxy.Subscribe();

            _stocks = _proxy.GetAllStocks();
            this.McDataGrid.ItemsSource = _stocks;

            this.Closing += (sender, args) =>
            {
                try
                {
                    _proxy?.Unsubscribe();
                }
                catch
                {
                    // ignored
                }
            };
        }

        public void PriceChange(string code, string name, decimal price)
        {
            if (_stocks != null)
            {
                var customerIndex = _stocks.IndexOf(_stocks.FirstOrDefault(c => c.Code == code));
                if (customerIndex >= 0)
                {
                    _stocks[customerIndex] = new Stock {Code = code, Name = name, Price = price };

                    this.McDataGrid.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                    {
                        this.McDataGrid.Items.Refresh();
                    }));
                }
            }
        }
    }
```

For more info about SqlTableDependency, refere to https://github.com/christiandelbianco/monitor-table-change-with-sqltabledependency 

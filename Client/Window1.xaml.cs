using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Threading;
using DataGridSample.PriceServiceProxy;

namespace DataGridSample
{
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
}
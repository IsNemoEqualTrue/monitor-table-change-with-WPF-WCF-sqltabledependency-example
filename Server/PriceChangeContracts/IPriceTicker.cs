using System.Collections.Generic;
using System.ServiceModel;
using ConsoleApplicationServer.Models;

namespace ConsoleApplicationServer.PriceChangeContracts
{
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
}
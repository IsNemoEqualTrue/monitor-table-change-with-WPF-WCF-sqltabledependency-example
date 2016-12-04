using System.ServiceModel;

namespace ConsoleApplicationServer.PriceChangeContracts
{
    public interface IPriceChangeCallBack
    {
        [OperationContract]
        void PriceChange(string code, string name, decimal price);
    }
}
using System;

namespace SoyBean.IAP
{
    public interface IAP
    {
        void Purchase(IAPItemData data, Action<bool> onCompleted);
        void Purchase(string storeId, Action<bool> onCompleted);
        string GetLocalPriceStr(string productId, string originPrice);
        void RestoreTransactions();
    }
}
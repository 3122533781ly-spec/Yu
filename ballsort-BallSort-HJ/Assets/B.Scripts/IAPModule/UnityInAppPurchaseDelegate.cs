using System;

namespace SoyBean.IAP
{
    public class UnityInAppPurchaseDelegate : IAP
    {
        public void Purchase(IAPItemData data, Action<bool> onCompleted)
        {
            //SDK
        }

        public void Purchase(string storeId, Action<bool> onCompleted)
        {
        }

        public string GetLocalPriceStr(string productId, string originPrice)
        {
            return "";
        }

        public void RestoreTransactions()
        {
        }
    }
}
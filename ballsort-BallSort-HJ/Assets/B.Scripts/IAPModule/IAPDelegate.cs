using System;

namespace SoyBean.IAP
{
    public static class IAPDelegate
    {
        public static void Purchase(IAPItemData data, Action<bool> onCompleted)
        {
            _delegate.Purchase(data, onCompleted);
        }

        public static void Purchase(string storeId, Action<bool> onCompleted)
        {
            _delegate.Purchase(storeId, onCompleted);
        }

        public static string GetLocalPriceStr(string productId, string originPrice)
        {
            return _delegate.GetLocalPriceStr(productId, originPrice);
        }

        public static void RestoreTransactions()
        {
            _delegate.RestoreTransactions();
        }

        static IAPDelegate()
        {
#if UNITY_EDITOR
            _delegate = new EditorIAPDelegate();
#else
            _delegate = new UnityInAppPurchaseDelegate();
#endif
        }

        private static IAP _delegate;
    }
}
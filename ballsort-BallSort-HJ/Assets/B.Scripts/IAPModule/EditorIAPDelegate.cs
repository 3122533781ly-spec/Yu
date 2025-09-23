using System;
using Lei31.Localizetion;

namespace SoyBean.IAP
{
    public class EditorIAPDelegate : IAP
    {
        public void Purchase(IAPItemData data, Action<bool> onCompleted)
        {
            if (data.ItemType == ShopItemType.WatchAD)
            {
                onCompleted.Invoke(true);
                // ADMudule.ShowRewardedAd(ADPosConst.ShopWatchAD, );
            }
            else if (data.Price < 0)
            {
                onCompleted.Invoke(true);
            }
            else
            {
                onCompleted.Invoke(true);
                FloatingWindow.Instance.Show(LocalizationManager.Instance.GetTextByTag(LocalizationConst.IAP_SUCCESS));
            }
        }

        public void Purchase(string storeId, Action<bool> onCompleted)
        {
            onCompleted.Invoke(true);
        }

        public string GetLocalPriceStr(string productId, string originPrice)
        {
            return originPrice;
        }

        public void RestoreTransactions()
        {
            LDebug.Log("Do Restore transaction in Editor");
        }
    }
}
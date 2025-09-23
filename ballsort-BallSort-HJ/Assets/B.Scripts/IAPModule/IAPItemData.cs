using System.Collections.Generic;
using ProjectSpace.Lei31Utils.Scripts.IAPModule;

namespace SoyBean.IAP
{
    [System.Serializable]
    public class IAPItemData : IConfig
    {
        public int IntID;
        public bool IsValid = true;
        public string TitleName;
        public string TitleNameTag;
        public float Price;
        public ShopItemType ItemType;
        public List<RewardData> Rewards;

        public bool IsMore;
        public int LimitPurchaseTime;//限制购买次数
        public int PurhcaseInterval;//购买间隔  min
        public string Remarks;
        public bool Tag_Double;
        public int Tag_Identify;

        public string StoreId;

        public IAPProductType Type;

        public int ID
        {
            get { return IntID; }
        }
    }

//same as Purchasing ProductType
    public enum IAPProductType
    {
        Consumable,
        NonConsumable,
        Subscription
    }

    public enum ShopItemType
    {
        Cash = 1,
        Coin = 2,
        WatchAD = 3,
        Share = 4,
        WatchManyAD = 5,
        Star = 6,
        Free = 7,
        WatchAdHaveCoolTIme = 8,
    }
}
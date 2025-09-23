using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Redeem
{
    [Serializable]
    public class RedeemCountryData : IConfig
    {
        public int id;
        public string countryCode;
        public string remark;
        public List<int> moneyConfigs;
        public List<int> giftCardConfigs;
        public int ID => id;

        public int GetMoneyConfig(int index)
        {
            return index < moneyConfigs.Count ? moneyConfigs[index] : moneyConfigs.Last();
        }
        public int GetGiftCardConfig(int index)
        {
            return index < giftCardConfigs.Count ? giftCardConfigs[index] : giftCardConfigs.Last();
        }
    }
}

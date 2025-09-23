using System;
using System.Collections.Generic;
using ProjectSpace.Lei31Utils.Scripts.Framework.App;
using UnityEngine;

namespace _02.Scripts.Config
{
    public class ConstantConfig : ScriptableConfigGroup<ConstantData, ConstantConfig>
    {

        public int GetOpenBoxNeedCoin()
        {
            return GetConfigByID((int) ConstantIDDef.OpenBoxNeedCoin).Value;
        }
        
        public int GetSameRewardCoin()
        {
            return GetConfigByID((int) ConstantIDDef.SameReward).Value;
        }
        
        public List<int> GetPassLevelCoin()
        {
            return GetConfigByID((int) ConstantIDDef.PassLevelCoin).ValueList;
        }

        public int GetBallPriceCoin()
        {
            return GetConfigByID((int)ConstantIDDef.BallCoin).Value;
        }
        
        public int GetSlotMciNeedStar()
        {
            return GetConfigByID((int)ConstantIDDef.SlotMciStar).Value;
        }

        public int GetTubePriceCoin()
        {
            return GetConfigByID((int)ConstantIDDef.TubeCoin).Value;
        }


        public int GetThemePriceCoin()
        {
            return GetConfigByID((int)ConstantIDDef.ThemeCoin).Value;
        }

        public int GetShopAdsCoin()
        {
            return GetConfigByID((int)ConstantIDDef.AdsGetCoin).Value;
        }
        
        public float GetNumberFromList(int id, float defaultv = 0)
        {
            if (TryGetConfigByID(id, out ConstantData data))
            {
                Debug.Log("分支：" + Game.Instance.GetSystem<RemoteControlSystem>().RedeemMenoyCount.Value);
                return data.ValueList[Game.Instance.GetSystem<RemoteControlSystem>().RedeemMenoyCount.Value];
            }
            return defaultv;
        }
        

        public int GetRemoveCoin()
        {
            return GetConfigByID((int)ConstantIDDef.RemoveAdCoin).Value;
        }
        
        
        public int GetInterpolationAd()
        {
            return GetConfigByID((int)ConstantIDDef.InterpolationAd).Value;
        }
        
        public int GetSpecialLevelNeedPass()
        {
            return GetConfigByID((int)ConstantIDDef.SpecialLevel).Value;
        }
        
        public int GetBigTurn()
        {
            return GetConfigByID((int)ConstantIDDef.BigTurn).Value;
        }
        public int GetGetMoneyLevel()
        {
            return GetConfigByID((int)ConstantIDDef.GetMoneyLevel).Value;
        }
    }

    [Serializable]
    public class ConstantData : IConfig
    {
        public int IntID;
        public int Value;
        public string Desc;
        public List<int> ValueList;
        public int ID => IntID;
    }

    public enum ConstantIDDef
    {
        BallCoin = 1,//时间道具增加时间数量
        TubeCoin =2,//复活所用金币
        ThemeCoin = 3,//时间通过金币
        AdsGetCoin = 4,
        OpenBoxNeedCoin = 5,//金币宝箱
        PassLevelCoin = 6,//通关金币
        SameReward = 7,//通关金币
        PassLevelMoney = 8,//通关金币
        SlotMciStar = 9,//老虎机需要星星
        RemoveAdCoin = 10,//去广告金币
        InterpolationAd = 11,//插屏广告出现关卡
        SpecialLevel = 12,//特殊关卡需要通过多少关
        BigTurn = 13,//大转盘次数
        GetMoneyLevel = 14,//固定关卡结束宝箱切换
    }
}
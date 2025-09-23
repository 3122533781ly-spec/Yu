using System;
using System.Collections.Generic;
using Fangtang.Utils;
using Newtonsoft.Json;
using Sirenix.Utilities;
using SoyBean.IAP;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _02.Scripts.Common
{
    /// <summary>
    /// 模拟服务器处理逻辑代码，下发服务器数据接口
    /// </summary>
    public class ServerLogic : Singleton<ServerLogic>
    {
        #region 通用方法

        public T GetProgress<T>(string key) where T : ProgressBase
        {
            
            return null;
            var serverData = SoyProfile.Get(key, "");
            try
            {
                Debug.Log($"Get:{key}:{serverData}");
                return JsonConvert.DeserializeObject<T>(serverData);
            }
            catch (Exception e)
            {
                Debug.LogError($"服务器数据错误:{key}:{serverData}");
                return null;
            }
        }

        public void SetServerProgress<T>(string key, T newProgress, bool isDelay) where T : ProgressBase
        {
            // if (key != newProgress.Key)
            // {
            //     Debug.LogError("上传错误的数据");
            // }
            //
            // var progress = JsonConvert.SerializeObject(newProgress);
            // SoyProfile.DelaySet(key, progress, isDelay ? 1 : 0);
        }

        #endregion

        #region 下发

        #endregion

        #region 同步服务器数据

        /// <summary>
        /// 跨天
        /// </summary>
        public void RefreshDay()
        {
        }


        public void RefreshWeek()
        {
        }

        #endregion


        #region NotUse

        private ServerLogic()
        {
        }

        #endregion
    }


    [Serializable]
    public enum RewardState
    {
        Lock = 0, //没解锁
        UnclaimedButUnlock = 1, //解锁可领取
        Claimed = 2, //已领取
    }

    public class ProgressBase
    {
        public string Key;

        public bool IsEmpty()
        {
            return Key.IsNullOrWhitespace();
        }
    }

    public class SigninProgress : ProgressBase
    {
        public int Day;
        public int IsClaim;

        public SigninProgress(int day, int isClaim)
        {
            Key = SoyProfileConst.SigninProgress;
            Day = day;
            IsClaim = isClaim;
        }
    }


    public class FirstChargeProgress : ProgressBase
    {
        public int Day;
        public bool IsClaim;

        public FirstChargeProgress(int day, bool isClaim)
        {
            Key = SoyProfileConst.FirstChargeProgress;
            Day = day;
            IsClaim = isClaim;
        }
    }


    public class CoinShopProgressData : ProgressBase
    {
        public int WatchCount;

        public CoinShopProgressData(int watchCount)
        {
            Key = SoyProfileConst.CoinShopProgress;
            WatchCount = watchCount;
        }
    }

    public class LimitTimePackProgress : ProgressBase
    {
        public long EndTime;
        public int CurrentIndex;
        public List<int> BuyCount;

        public LimitTimePackProgress(long endTime, int currentIndex, List<int> buyCount)
        {
            Key = SoyProfileConst.LimitTimeProgress;
            EndTime = endTime;
            CurrentIndex = currentIndex;
            BuyCount = buyCount;
        }
    }


    public class PurchaseTimeProgress : ProgressBase
    {
        public int PurchaseTime;

        public PurchaseTimeProgress(int purchaseTime)
        {
            Key = SoyProfileConst.PurchaseTime;
            PurchaseTime = purchaseTime;
        }
    }

    public class OnlineTimeProgress : ProgressBase
    {
        public List<int> BuyCount;
        public List<int> WatchCount;
        public List<ShopItemType> Type;
        public int OnlineTime;

        public OnlineTimeProgress(List<int> buyCount, List<int> watchCount, List<ShopItemType> type, int onlineTime)
        {
            Key = SoyProfileConst.OnlineTimeProgress;
            BuyCount = buyCount;
            WatchCount = watchCount;
            Type = type;
            OnlineTime = onlineTime;
        }
    }


    public class LevelChallengeProgress : ProgressBase
    {
        public List<int> BuyCount;
        public int SpecialFoodId;
        public int Progress;

        public LevelChallengeProgress(List<int> buyCount, int specialFoodId, int progress)
        {
            Key = SoyProfileConst.LevelChallengeProgress;
            BuyCount = buyCount;
            SpecialFoodId = specialFoodId;
            Progress = progress;
        }
    }

    public class GameSkinProgress : ProgressBase
    {

        public List<int> ballValue = new List<int>();

        public List<int> tubeValue = new List<int>();

        public List<int> themeValue = new List<int>();

        public int useBallId;

        public int useTubeId;

        public int useThemeId;

        public GameSkinProgress(List<int> ballUnLock, List<int> tubeUnLock, List<int> themeUnLock, int useBall, int useTube, int useTheme)
        {
            Key = SoyProfileConst.GameSkinProgress;
            ballValue = ballUnLock;
            tubeValue = tubeUnLock;
            themeValue = themeUnLock;
            useBallId = useBall;
            useTubeId = useTube;
            useThemeId = useTheme;
        }
    }

    public class GameToolProgress : ProgressBase
    {
        public int Coin;
        public int Star;
        public int ManualValue;

        public int AddPipe;
        public int RevocationTool;
        public int MergeFoodTool;
        public int RefreshFoodTool;
        public int RebackTool;
        public float Money;
        public int Diamond;

        public GameToolProgress(int coin, int star, int manualValue, int addPipe, int revocationTool,float money,int diamond)
        {
            Key = SoyProfileConst.GameToolProgress;
            Coin = coin;
            Star = star;
            ManualValue = manualValue;
            AddPipe = addPipe;
            RevocationTool = revocationTool;
            Money = money;
            Diamond = diamond;
        }

    }

    public class ChestProgress : ProgressBase
    {
        public string LevelChestProgress;
        public string StarChestProgress;

        public ChestProgress(string levelChestProgress, string starChestProgress)
        {
            Key = SoyProfileConst.ChestProgress;
            LevelChestProgress = levelChestProgress;
            StarChestProgress = starChestProgress;
        }
    }
}
using _02.Scripts.Config;
using ProjectSpace.Lei31Utils.Scripts.Framework.App;
using ProjectSpace.Lei31Utils.Scripts.Framework.ElementKit;
using ProjectSpace.Lei31Utils.Scripts.IAPModule;
using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;
using Redeem;
using UnityEngine;

public class DebugPlayer : IDebugPage
{
    public string Title
    {
        get { return "Player"; }
    }

    public void Draw()
    {
        GUILayout.Label($"language:{LocalizationManager.Instance.Model.CurrentLanguage.LanguageName}");

        if (GUILayout.Button("Coin * 1000"))
        {
            Game.Instance.CurrencyModel.AddGoodCount(GoodType.Coin, 0, 1000);
        }

        if (GUILayout.Button("RevocationTool * 10"))
        {
            var reward = new RewardData(GoodType.Tool, 10, (int)GoodSubType.RevocationTool);
            RewardClaimHandle.ClaimReward(reward, "ad", IapStatus.Free);
        }

        if (GUILayout.Button("AddPipe * 10"))
        {
            var reward = new RewardData(GoodType.Tool, 10, (int)GoodSubType.AddPipe);
            RewardClaimHandle.ClaimReward(reward, "ad", IapStatus.Free);
        }
        if (GUILayout.Button("球排序胜利"))
        {
            if (inGame2 == null)
            {
                inGame2 = SceneElementManager.Instance.Resolve<InGame>();
            }
            inGame2.Win();
        }
        if (GUILayout.Button("打开礼物弹窗"))
        {
            Game.Instance.LevelModel.RestartStoreGold();

            var randomReward = InGameRewardConfig.Instance.GetRandomRewardData();
            RewardClaimHandle.ClaimReward(randomReward, "InGameBox", IapStatus.Free);

            DialogManager.Instance.GetDialog<DressUpDialog>().ShowGetSkinUI(randomReward.goodType,
                randomReward.count, randomReward.subType, isForce: true);
        }

        if (GUILayout.Button("停止BGM"))
        {
            AudioManager.Instance.StopBGM();
        }

        if (GUILayout.Button("缓存清除"))
        {
            SoyProfile.DelaySet(SoyProfileConst.NormalLevel, SoyProfileConst.NormalLevel_Default, 1);
            SoyProfile.DelaySet(SoyProfileConst.SpecialLevel, SoyProfileConst.NormalLevel_Default, 1);
            SoyProfile.DelaySet(SoyProfileConst.HaveCoin, SoyProfileConst.HaveCoinDefault, 1);
            SoyProfile.DelaySet(SoyProfileConst.ADRemoved, SoyProfileConst.ADRemoved_Default, 1);
            SoyProfile.DelaySet(SoyProfileConst.CoinShopProgress, SoyProfileConst.CoinShopProgressDefault, 1);
            SoyProfile.DelaySet(SoyProfileConst.PurchaseTime, SoyProfileConst.PurchaseTimeDefault, 1);
            SoyProfile.DelaySet(SoyProfileConst.GameToolProgress, SoyProfileConst.GameToolProgressDefault, 1);

            PlayerPrefs.DeleteAll();
        }

        if (GUILayout.Button("Open Redeem"))
        {
            Game.Instance.Model.CanRedeem.Value = true;
            Game.Instance.isSDKInitCompleted = true;
        }
        if (GUILayout.Button("连线关卡跳过"))
        {
            if (_inGame == null)
            {
                _inGame = SceneElementManager.Instance.Resolve<InGameLineBee>();
            }
            _inGame.GameWin();
        }
        if (GUILayout.Button("只进入B面"))
        {
            Game.Instance.Model.IsOnlyB.Value = true;
        }
        if (GUILayout.Button("只进入ss面"))
        {
            LineBee.Instance.LevelModel.MaxUnlockLevel.Value = 69;
        }

        if (GUILayout.Button("+ $10"))
        {
            Game.Instance.GetSystem<RedeemSystem>().ChangeMoney(10f);
        }

        if (GUILayout.Button("+ AD 10"))
        {
            Game.Instance.GetSystem<RedeemSystem>().AddWatchADCount(10);
        }

        if (GUILayout.Button("+ Play game 10"))
        {
            Game.Instance.GetSystem<RedeemSystem>().AddPlayGameCount(10);
        }

        if (GUILayout.Button("跳过正在进行的提现任务"))
        {
            Game.Instance.GetSystem<RedeemSystem>().JumpNowCondition();
        }

        if (GUILayout.Button("AB Test: 1"))
        {
            Game.Instance.GetSystem<RemoteControlSystem>().RedeemMenoyCount.Value = 0;
        }

        if (GUILayout.Button("AB Test: 2"))
        {
            Game.Instance.GetSystem<RemoteControlSystem>().RedeemMenoyCount.Value = 1;
        }

        if (GUILayout.Button("AB Test: 3"))
        {
            Game.Instance.GetSystem<RemoteControlSystem>().RedeemMenoyCount.Value = 2;
        }

        if (GUILayout.Button("设置99刀"))
        {
            Game.Instance.CurrencyModel.SetCurrentMoney(99.9f);
        }
        if (GUILayout.Button("增加10000钻石"))
        {
            Game.Instance.CurrencyModel.AddGoodCount(GoodType.Gem, 0, 10000);
        }

        if (GUILayout.Button("star +10"))
        {
            Game.Instance.CurrencyModel.AddGoodCount(GoodType.Star, 0, 10);
        }

        if (GUILayout.Button("打开老虎机弹窗"))
        {
            DialogManager.Instance.ShowDialog(DialogName.SlotMciDialog);
        }
        if (GUILayout.Button("进入球排序游戏"))
        {
            TransitionManager.Instance.Transition(0.5f, () => { SceneManager.LoadScene("InGameScenario"); },
               0.5f);
        }

        if (GUILayout.Button("打开评分弹窗"))
        {
            DialogManager.Instance.ShowDialog(DialogName.RatingSimpleDialog);
            //   DialogManager.Instance.ShowDialog(DialogName.RatingStarDialog);
        }

        if (GUILayout.Button("打开提现信息弹窗"))
        {
            DialogManager.Instance.ShowDialog(DialogName.RedeemInfoDialog);
        }

        GUILayout.Label($"UserPurchaseHabit:{Game.Instance.GetSystem<UserPurchaseHabitSystem>().CurrentType}");
        GUILayout.Label(Game.Instance.GetSystem<UserPurchaseHabitSystem>().Habit.ToString());
    }

    private InGameLineBee _inGame;
    private InGame inGame2;
}
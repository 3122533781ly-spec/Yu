using System.Collections.Generic;
using _02.Scripts.Config;
using ProjectSpace.Lei31Utils.Scripts.Framework.App;
using ProjectSpace.Lei31Utils.Scripts.IAPModule;
using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;
using UnityEngine;
using UnityEngine.UI;

namespace _02.Scripts.InGame.UI
{
    public class InGameSuccessUI : ElementUI<global::InGame>
    {
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Text coinText;
        [SerializeField] private InGameBoxReward boxReward;
        private RewardData _coinData;
        private RewardData _skinData;

        private void OnEnable()
        {
            _coinData = Context.CellMapModel.LevelData.GetRandomCoin();
            nextLevelButton.onClick.AddListener(NextLevelButton);
            coinText.text = $"X{_coinData.count}";
            boxReward.Init(_coinData);

            boxReward.watchAdButton.SetActiveVirtual(Game.Instance.LevelModel.GetRate(_coinData.count) >= 100);
            AudioClipHelper.Instance.PlaySound(AudioClipEnum.Win);
        }

        private void OnDisable()
        {
            nextLevelButton.onClick.RemoveListener(NextLevelButton);
            if (Game.Instance.LevelModel.GetRate(_coinData.count) >= 100)
            {
                Game.Instance.LevelModel.RestartStoreGold();
            }
        }

        private void NextLevelButton()
        {
            Deactivate();

            Game.Instance.LevelModel.AddStoreGold(_coinData.count);
            CoinFlyAnim.Instance.Play(_coinData.count, nextLevelButton.transform.position, AnimIconType.Coin,
                () =>
                {
                    RewardClaimHandle.ClaimReward(_coinData, "InGameSuccess", IapStatus.Free);

                    //是网赚并且是最大关卡 || 是网赚并且是特殊关卡直接弹出
                    if ((Game.Instance.Model.IsWangZhuan() &&
                         Game.Instance.LevelModel.EnterLevelID == Game.Instance.LevelModel.MaxUnlockLevel.Value - 1 &&
                         Game.Instance.LevelModel.CopiesType == CopiesType.Thread) ||
                        Game.Instance.LevelModel.CopiesType == CopiesType.SpecialLevel)
                    {
                        //美元结算弹窗控制
                        if (Game.Instance.CurrencyModel.GetCurrentMoney() <
                            ConstantConfig.Instance.GetGetMoneyLevel())
                        {
                            _context.GetView<InGameWinReward>().Activate();
                        }
                        else
                        {
                            _context.GetView<InGameBoxDialog>().Activate();
                        }
                    }
                    else
                    {
                        EnterNextLevel();
                    }
                });
        }


        private void EnterNextLevel()
        {
            if (Game.Instance.LevelModel.CopiesType == CopiesType.Thread)
            {
                if (Game.Instance.LevelModel.EnterLevelID == Game.Instance.LevelModel.MaxUnlockLevel.Value)
                {
                    Game.Instance.RestartGame("NextLevel", Game.Instance.LevelModel.EnterLevelID,
                        forceShowAd: !Game.Instance.Model.IsWangZhuan());
                    if (DialogManager.Instance.GetDialog<LevelUIDialog>() != null)
                        DialogManager.Instance.GetDialog<LevelUIDialog>().PassLastLevel();
                }
                else
                {
                    Game.Instance.RestartGame("NextLevel", Game.Instance.LevelModel.EnterLevelID + 1,
                        forceShowAd: !Game.Instance.Model.IsWangZhuan());
                }
            }
            else if (Game.Instance.LevelModel.CopiesType == CopiesType.SpecialLevel)
            {
                Game.Instance.RestartGame("NextLevel", Game.Instance.LevelModel.EnterLevelID,
                    forceShowAd: !Game.Instance.Model.IsWangZhuan());
            }
        }
    }
}
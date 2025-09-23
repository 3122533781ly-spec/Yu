using System.Collections.Generic;
using _02.Scripts.Config;
using _02.Scripts.DressUpUI;
using _02.Scripts.InGame.UI;
using ProjectSpace.Lei31Utils.Scripts.Framework.App;
using ProjectSpace.Lei31Utils.Scripts.IAPModule;
using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;
using UnityEngine;
using UnityEngine.UI;

namespace _02.Scripts.InGame
{
    public class InGameBoxDialog : ElementUI<global::InGame>
    {
        [SerializeField] private Button adButton;
        [SerializeField] private Button receiveButton;
        private List<MoneyReward> _rewardList;
        private MoneyReward _coinData;
        private MoneyReward _starData;
        private bool _isWatch = false;

        private void OnEnable()
        {
            adButton.onClick.AddListener(WatchAd);
            receiveButton.onClick.AddListener(NextLevelButton);
        }

        private void OnDisable()
        {
            adButton.onClick.RemoveAllListeners();
            receiveButton.onClick.RemoveAllListeners();
        }

        public override void Activate()
        {
            base.Activate();
            _isWatch = false;
            _rewardList = new List<MoneyReward>();
            if (Game.Instance.Model.IsWangZhuan())
            {
                if (DollarClearDataConfig.Instance.IsCash())
                {
                    _coinData = new MoneyReward
                    { goodType = GoodType.Money, moneyCount = DollarClearDataConfig.Instance.GetAwardCash() };
                }
                else
                {
                    _coinData = new MoneyReward
                    { goodType = GoodType.Gem, count = DollarClearDataConfig.Instance.GetAwardGem() };
                }
            }

            _starData = new MoneyReward { goodType = GoodType.Star, count = 3 };
            _rewardList.Add(_coinData);
            _rewardList.Add(_starData);
            adButton.SetActiveVirtual(true);
            Refresh();
        }

        private void Refresh()
        {
            adButton.SetActiveVirtual(!_isWatch);
        }

        private void WatchAd()
        {
            ADMudule.ShowRewardedAd("WatchAd_GetRevocationTool", (isSuccess) =>
            {
                if (isSuccess)
                {
                    _isWatch = true;
                    PlayAnimeAndClose();
                }
            });
        }

        private void NextLevelButton()
        {
            Deactivate();
            ADMudule.ShowRewardAdByLevel("InGameMoneyBox_Skip", ret => { StarAnime(false); });
        }

        private void PlayAnimeAndClose()
        {
            Deactivate();
            adButton.SetActiveVirtual(false);
            DialogManager.Instance.ShowDialogWithContext(DialogName.GiftClaimDialog,
                new GiftClaimDialogContext
                {
                    CurrentGoodsData = _coinData,
                    TopRectTransform = (RectTransform)CoinFlyAnim.Instance.GetTargetIconRect(AnimIconType.Money),
                    HasWatchedADj = _starData.goodType == GoodType.Money,
                    CloseAction = () => { StarAnime(false); }
                });
            // StarAnime(true);
        }

        private void CoinAnime()
        {
            CoinFlyAnim.Instance.Play(_coinData.GetFlyAnimeCount(), receiveButton.transform.position,
                AnimIconType.Money, () =>
                {
                    RewardClaimHandle.ClaimReward(_coinData, "GameWin", IapStatus.Free);
                    CheckAnimeOver();
                });
        }

        private void CheckAnimeOver()
        {
            JobUtils.Delay(0.5f, () =>
            {
                CheckIsShowRedeemInfoDialog();
                CheckShowSlotDialog();
                Game.Instance.NotUseCode();
                Game.Instance.GetSystem<RatingSystem>().CheckAndShowRating();
            });
        }

        private void CheckIsShowRedeemInfoDialog()
        {
            if (Game.Instance.Model.IsWangZhuan() && Game.Instance.LevelModel.EnterLevelID ==
                Game.Instance.LevelModel.MaxUnlockLevel.Value - 1 && Game.Instance.LevelModel.MaxUnlockLevel.Value == 2 &&
                !CPlayerPrefs.GetBool("isShowRedeemInfo", false))
            {
                DialogManager.Instance.ShowDialog(DialogName.RedeemInfoDialog);
            }
        }

        private void StarAnime(bool isPlayMoney)
        {
            CoinFlyAnim.Instance.Play(_starData.GetFlyAnimeCount(), receiveButton.transform.position,
                AnimIconType.Star, () =>
                {
                    RewardClaimHandle.ClaimReward(_starData, "GameWin", IapStatus.Free);
                    if (isPlayMoney)
                    {
                        CoinAnime();
                    }
                    else
                    {
                        CheckAnimeOver();
                    }
                });
        }

        private void CheckShowSlotDialog()
        {
            if (Game.Instance.CurrencyModel.CanShowSlotDialog())
            {
                DialogManager.Instance.ShowDialog(DialogName.SlotMciDialog);
            }
        }
    }
}
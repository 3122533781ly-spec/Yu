using _02.Scripts.Config;
using _02.Scripts.Util;
using DG.Tweening;
using ProjectSpace.Lei31Utils.Scripts.Framework.App;
using ProjectSpace.Lei31Utils.Scripts.IAPModule;
using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;
using UnityEngine;
using UnityEngine.UI;

namespace _02.Scripts.InGame.UI
{
    public class InGameBoxReward : MonoBehaviour
    {
        [SerializeField] private RectTransform rewardIconRectTransform;
        [SerializeField] private Text progress;
        [SerializeField] public Button watchAdButton;
        private RewardData _coinData;

        private void OnEnable()
        {
            watchAdButton.onClick.AddListener(WatchAdButton);

            JobUtils.Delay(0.4f, () => { Refresh(_coinData); });
        }

        private void OnDisable()
        {
            watchAdButton.onClick.RemoveListener(WatchAdButton);
        }

        public void Init(RewardData data)
        {
            _coinData = data;
            var startNumber = Game.Instance.LevelModel.GetRate();
            var startH = 460 * Game.Instance.LevelModel.GetRate(0, false);

            progress.text = $"{startNumber}%";

            rewardIconRectTransform.SetSizeDeltaY(startH);
        }

        public void Refresh(RewardData rewardData)
        {
            var startNumber = Game.Instance.LevelModel.GetRate();
            var endNumber = Game.Instance.LevelModel.GetRate(rewardData.count);

            var endH = 436 * Game.Instance.LevelModel.GetRate(rewardData.count, false);

            UtilClass.DoNumber(progress, startNumber, endNumber, 1, "%");
            rewardIconRectTransform.DOSizeDelta(new Vector2(rewardIconRectTransform.sizeDelta.x, endH), 1);
        }

        private void WatchAdButton()
        {
            if (Game.Instance.LevelModel.GetRate(_coinData.count) < 100)
            {
                return;
            }

            ADMudule.ShowRewardedAd("InGameAddItemUI_ClickAD", (isSuccess) =>
            {
                if (isSuccess)
                {
                    Game.Instance.LevelModel.RestartStoreGold();
                    Init(_coinData);
                    var randomReward = InGameRewardConfig.Instance.GetRandomRewardData();
                    RewardClaimHandle.ClaimReward(randomReward, "InGameBox", IapStatus.Free);

                    DialogManager.Instance.GetDialog<DressUpDialog>().ShowGetSkinUI(randomReward.goodType,
                        randomReward.count, randomReward.subType, isForce: true);
                    watchAdButton.SetActiveVirtual(false);
                }
            });
        }
    }
}
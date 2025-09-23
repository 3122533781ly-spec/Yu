using _02.Scripts.DressUpUI;
using ProjectSpace.Lei31Utils.Scripts.Framework.App;
using ProjectSpace.Lei31Utils.Scripts.IAPModule;
using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;
using UnityEngine;
using UnityEngine.UI;

namespace _02.Scripts.InGame
{
    public class FristInGameReward : ElementUI<global::InGame>
    {
        [SerializeField] private Button openButton;

        private void OnEnable()
        {
            openButton.onClick.AddListener(GetTenMoney);
        }

        private void OnDisable()
        {
            openButton.onClick.RemoveListener(GetTenMoney);
        }

        //第一次进入游戏给$10
        public void CheckIsShow()
        {
            if (Game.Instance.LevelModel.MaxUnlockLevel.Value == 1 &&
                Game.Instance.CurrencyModel.GetCurrentMoney() == 0 && Game.Instance.Model.IsWangZhuan()) //
            {
               // Activate();
            }
            else
            {
                Deactivate();
            }
        }

        private void GetTenMoney()
        {
            Deactivate();

            DialogManager.Instance.ShowDialogWithContext(DialogName.GiftClaimDialog,
                new GiftClaimDialogContext
                {
                    CurrentGoodsData = new MoneyReward {moneyCount = 10, goodType = GoodType.Money},
                    TopRectTransform = (RectTransform) CoinFlyAnim.Instance.GetTargetIconRect(AnimIconType.Money),
                    CloseAction = () => { }
                });
        }
    }
}
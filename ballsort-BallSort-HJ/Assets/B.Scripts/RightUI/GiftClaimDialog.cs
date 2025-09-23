using System;
using _02.Scripts.Config;
using _02.Scripts.Home.Active;
using _02.Scripts.Util;
using ProjectSpace.BubbleMatch.Scripts.Util;
using ProjectSpace.Lei31Utils.Scripts.Framework.App;
using ProjectSpace.Lei31Utils.Scripts.IAPModule;
using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;
using UnityEngine;
using UnityEngine.UI;

namespace _02.Scripts.DressUpUI
{
    public class GiftClaimDialogContext : DialogContent
    {
        public MoneyReward CurrentGoodsData;
        public RectTransform TopRectTransform;
        public bool HasWatchedADj;
        public Action CloseAction;
    }

    public class GiftClaimDialog : Dialog<GiftClaimDialogContext>
    {
        [SerializeField] private Image icon;
        [SerializeField] private Text numberTxt;
        [SerializeField] private Text2DOutline numberTxtOutline;
        [SerializeField] private SmartLocalizedText doubleAdTxt;
        [SerializeField] private Button adBtn;
        [SerializeField] private Button claimBtn;
        private bool _isLookAd = false;

        public override void ShowDialog()
        {
            Activate(91 + 10000);
        }

        public override void ShowDialogWithContext(GiftClaimDialogContext outContent)
        {
            base.ShowDialogWithContext(outContent);
            _isLookAd = false;
            FreshGoods();
            claimBtn.onClick.AddListener(ClaimGoods);
            adBtn.onClick.AddListener(WatchAdDoubleReward);
        }

        public override void CloseDialog()
        {
            base.CloseDialog();
            claimBtn.onClick.RemoveAllListeners();
            adBtn.onClick.RemoveAllListeners();
        }

        /// <summary>
        /// 刷新商品
        /// </summary>
        /// <param name="data"></param>
        private void FreshGoods()
        {
            numberTxt.text = $"X{Content.CurrentGoodsData.count}";
            switch (Content.CurrentGoodsData.goodType)
            {
                case GoodType.Money: //美元
                    numberTxt.text = "+$" + Content.CurrentGoodsData.GetCount().ToString("f2");
                    numberTxt.color = UtilClass.HexToColor("#44f63f");
                    numberTxtOutline.Refresh(UtilClass.HexToColor("#10750d"), 4);
                    break;

                default:
                    numberTxt.color = UtilClass.HexToColor("#FFFFFF");
                    numberTxtOutline.Refresh(UtilClass.HexToColor("#000000"), 6);
                    break;
            }

            //  doubleAdTxt.SetLocalizedText("Txt372", "x2");
            icon.sprite =
                SpriteManager.Instance.GetGoodTypeIconByType(Content.CurrentGoodsData.goodType,
                    Content.CurrentGoodsData.subType);
            adBtn.gameObject.SetActive(Content.HasWatchedADj && !_isLookAd);
        }

        private void WatchAdDoubleReward()
        {
            ADMudule.ShowRewardedAd("slotMci", ret =>
            {
                if (ret)
                {
                    _isLookAd = true;
                    switch (Content.CurrentGoodsData.goodType)
                    {
                        case GoodType.Money:
                            Content.CurrentGoodsData.moneyCount *= 2;
                            break;

                        default:
                            Content.CurrentGoodsData.count *= 2;
                            break;
                    }

                    FreshGoods();
                }
            });
        }

        /// <summary>
        /// 收取物品
        /// </summary>
        private void ClaimGoods()
        {
            if (Content.HasWatchedADj)
            {
                switch (Content.CurrentGoodsData.goodType)
                {
                    case GoodType.Money: //美元
                        CoinFlyAnim.Instance.Play(Content.CurrentGoodsData.GetFlyAnimeCount(),
                            claimBtn.transform.position,
                            Content.TopRectTransform.position, AnimIconType.Money,
                            () =>
                            {
                                Content.CloseAction?.Invoke();
                                Game.Instance.CurrencyModel.RewardMoney(Content.CurrentGoodsData.GetCount());
                            });
                        break;

                    default:
                        // GoodsConfig.Instance.AddGoods(currentGoodsData, times);
                        break;
                }
            }
            else
            {
                switch (Content.CurrentGoodsData.goodType)
                {
                    case GoodType.Money: //美元
                        CoinFlyAnim.Instance.Play((int)(Content.CurrentGoodsData.GetCount() * 100),
                            claimBtn.transform.position,
                            Content.TopRectTransform.position, AnimIconType.Money,
                            () =>
                            {
                                Content.CloseAction?.Invoke();
                                Game.Instance.CurrencyModel.RewardMoney(Content.CurrentGoodsData.GetCount());
                            });
                        break;

                    case GoodType.Gem: //美元
                        CoinFlyAnim.Instance.Play(Content.CurrentGoodsData.GetFlyAnimeCount(),
                            claimBtn.transform.position,
                            Content.TopRectTransform.position, AnimIconType.GemFlyObject,
                            () =>
                            {
                                Content.CloseAction?.Invoke();
                                Game.Instance.CurrencyModel.AddGoodCount(GoodType.Gem, 0,
                                    Content.CurrentGoodsData.count);
                            });
                        break;

                    case GoodType.Tool:
                        RewardClaimHandle.ClaimReward(Content.CurrentGoodsData, "SlotMic", IapStatus.Free);
                        Content.CloseAction?.Invoke();
                        break;

                    default:
                        CoinFlyAnim.Instance.Play(Content.CurrentGoodsData.GetFlyAnimeCount(),
                            claimBtn.transform.position,
                            CoinFlyAnim.Instance.GetIconType(Content.CurrentGoodsData.goodType),
                            () =>
                            {
                                Content.CloseAction?.Invoke();
                                RewardClaimHandle.ClaimReward(Content.CurrentGoodsData, "SlotMic", IapStatus.Free);
                                //    Debug.Log("数据为" + Content.CurrentGoodsData);
                            });
                        break;
                }
            }

            CloseDialog();
        }
    }
}
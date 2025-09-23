using System.Collections;
using System.Collections.Generic;
using _02.Scripts.Config;
using _02.Scripts.DressUpUI;
using ProjectSpace.Lei31Utils.Scripts.IAPModule;
using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;
using UnityEngine;
using UnityEngine.UI;

public class SlotMciDialog : Dialog
{
    [SerializeField] private Button claimButton;
    [SerializeField] private Text numberText;
    [SerializeField] private SwapImage claimSwapImage;
    [SerializeField] private Image ADImage;
    [SerializeField] private List<GoodsRoller> goodsRollers;

    [SerializeField] private float startPoint;
    [SerializeField] private float endPoint;

    private int playTimes = 0;

    [SerializeField] private float rollSpaceTime = 0.2f;
    [SerializeField] private float waitShowGoodsTime = 3f;
    [SerializeField] private RectTransform top;
    [SerializeField] private RectTransform gemTop;

    private SlotMciData hitSlotMciData;
    private GoodsData hitGoodsData;

    private System.Action downCallback = null;
    private int _maxPlayTime = 4;

    private void OnEnable()
    {
        claimButton.onClick.AddListener(OnClaimClicked);
    }

    private void OnDisable()
    {
        playTimes = 0;
        claimButton.onClick.RemoveListener(OnClaimClicked);

        downCallback?.Invoke();
        downCallback = null;
    }

    public override void ShowDialog()
    {
        base.ShowDialog();
        RestGoodsRollers();
        FreshUI();
        Game.Instance.CurrencyModel.ConsumeGoodNumber(GoodType.Star, 0,
            ConstantConfig.Instance.GetSlotMciNeedStar());
    }

    /// <summary>
    /// 重置转盘
    /// </summary>
    private void RestGoodsRollers()
    {
        hitSlotMciData = SlotMciDataConfig.Instance.GetHitSlotMciData();
        hitGoodsData = hitSlotMciData.GetGoodsData();
        Debug.Log($"Slot: {hitSlotMciData.id}, 物品：{hitGoodsData.Type}_{hitGoodsData.subType}");
        var all = SlotMciDataConfig.Instance.GetAllGoodsData();
        foreach (var good in goodsRollers)
        {
            good.ResetGoods(hitGoodsData, all);
            good.transform.SetLocalPositionY(startPoint);
        }
    }

    /// <summary>
    /// 刷新UI
    /// </summary>
    private void FreshUI()
    {
        // claimButton.gameObject.SetActive(HasLastTimes());
        var state = IapStatus.Free;

        if (playTimes == _maxPlayTime)
        {
            state = IapStatus.SoldAll;
            //  ADImage.SetActive(false);
        }
        else if (playTimes > 0 && playTimes < _maxPlayTime)
        {
            state = IapStatus.CanWatch;
            //   ADImage.SetActive(true);
        }
        else if (playTimes == 0)
        {
            state = IapStatus.Free;
            // ADImage.SetActive(false);
        }

        claimSwapImage.Set(state);
    }

    /// <summary>
    /// 开始老虎机
    /// </summary>
    private void StartRollPollers()
    {
        playTimes++;
        FreshUI();
        StartCoroutine(StartRollPollersInback());
    }

    private IEnumerator StartRollPollersInback()
    {
        foreach (var item in goodsRollers)
        {
            item.StartRolling(endPoint);
            yield return new WaitForSeconds(rollSpaceTime);
        }

        yield return new WaitForSeconds(waitShowGoodsTime);
        ClaimGoods();
    }

    /// <summary>
    /// 是否还有次数
    /// </summary>
    /// <returns></returns>
    private bool HasLastTimes()
    {
        return playTimes < _maxPlayTime;
    }

    private void OnClaimClicked()
    {
        if (HasLastTimes())
        {
            //第一次免费抽奖
            if (playTimes == 0)
            {
                StartRollPollers();
            }
            else
            {
                ADMudule.ShowRewardedAd("SlotMciDialog", ret =>
                {
                    if (ret)
                    {
                        RestGoodsRollers();
                        StartRollPollers();
                    }
                });
            }
        }
        else
        {
            Debug.LogError("次数一键用完");
        }
    }

    /// <summary>
    /// 领取物品
    /// </summary>
    private void ClaimGoods()
    {
        // var topos =
        // switch (hitGoodsData.Type)
        // {
        //     case GoodType.Gem:
        //     case GoodType.Money:
        //         // HomeAutoShowController.Add(hitGoodsData.Type, 0);
        //         break;
        //     default:
        //         break;
        // }

        var reward = new MoneyReward
        {
            goodType = hitGoodsData.Type,
            subType = hitGoodsData.subType,
        };

        if (hitSlotMciData.goodType == GoodType.Money)
        {
            var count = GetRandomMoney();
            reward.moneyCount = count;
        }
        else
        {
            reward.count = hitSlotMciData.count;
        }

        DialogManager.Instance.ShowDialogWithContext<GiftClaimDialogContext>(DialogName.GiftClaimDialog,
            new GiftClaimDialogContext
            {
                CurrentGoodsData = reward,
                TopRectTransform = hitSlotMciData.goodType == GoodType.Money ? top : gemTop,
                HasWatchedADj = hitSlotMciData.goodType == GoodType.Money
            });
    }

    /// <summary>
    /// 可能随机到0.01以下的数
    /// </summary>
    /// <returns></returns>
    private float GetRandomMoney()
    {
        var res = (ConstantConfig.Instance.GetNumberFromList(8) -
                   Game.Instance.CurrencyModel.GetCurrentMoney()) *
                  hitSlotMciData.rateAmount;
        if (res <= 0)
        {
            res = 0.01f;
        }

        return res;
    }
}
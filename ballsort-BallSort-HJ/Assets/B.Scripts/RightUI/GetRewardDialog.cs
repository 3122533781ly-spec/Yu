using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;
using Spine;
using System.Collections;
using System.Collections.Generic;
using _02.Scripts.Config;
using ProjectSpace.Lei31Utils.Scripts.Framework.App;
using ProjectSpace.Lei31Utils.Scripts.IAPModule;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GetRewardDialog : Dialog
{
    public Image icon;
    private GoodType type;
   

    [SerializeField] private Button useBtn;
    [SerializeField] private Button nextBtn;
    [SerializeField] private Text numberTxt;
    [SerializeField] private RectTransform iconTransform;
    [SerializeField] private Image claimAdImage;
    [SerializeField] private Text countText;

    public static bool hasWatchedAD = false;
    private GoodsData currentGoodsData;
    private System.Action downCallback = null;
    private float rateAmount = 1f;

    private void OnEnable()
    {
       // useBtn.onClick.AddListener(UseSkin);
       // nextBtn.onClick.AddListener(CloseSkinUI);
    }

    private void OnDisable()
    {
      //  useBtn.onClick.RemoveListener(UseSkin);
       // nextBtn.onClick.RemoveListener(CloseSkinUI);
    }

    public void Init(GoodType type, Sprite skinSprite, int subType)
    {
        this.type = type;
        icon.sprite = skinSprite;
        if (type == GoodType.Tool || type == GoodType.Coin)
        {
            iconTransform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            numberTxt.SetActiveVirtual(true);
         //   numberTxt.text = $"X{id}";
            useBtn.SetActiveVirtual(false);
        }
        else if (type == GoodType.SkinBall || type == GoodType.SkinTube || type == GoodType.SkinTheme)
        {
            iconTransform.localScale = new Vector3(1f, 1f, 1f);
            //  icon.SetNativeSize();
            useBtn.SetActiveVirtual(true);
            numberTxt.SetActiveVirtual(false);
        }
    }



    public void CloseSkinUI()
    {
       // DialogManager.Instance.GetDialog<GetNewSkinDialog>().CloseDialog();
    }
    
    /// <summary>
    /// 显示商品
    /// </summary>
    /// <param name="goodsId"></param>
    /// <param name="goodSubType"></param>
    /// <param name="hasAd"></param>
    /// <param name="dwon"></param>
    /// <param name="rateAmount"></param>
    public void Show(GoodType goodsId, int goodSubType, bool hasAd, System.Action dwon, float rateAmount)
    {
        Activate();
        hasWatchedAD = false;
        this.rateAmount = rateAmount;
        downCallback = dwon;
        FreshGoods(GoodsConfig.Instance.FindData(goodsId, goodSubType));
        claimAdImage.gameObject.SetActive(hasAd);

        float count;
        switch (currentGoodsData.Type)
        {
            case GoodType.Money: //美元
                countText.gameObject.SetActive(true);
                count = (ConstantConfig.Instance.GetNumberFromList(8) - Game.Instance.CurrencyModel.GetCurrentMoney()) *
                        this.rateAmount;
                countText.text = "+" + count.ToString("f2");
                break;

            default:
                countText.gameObject.SetActive(false);
                break;
        }
    }

    /// <summary>
    /// 刷新商品
    /// </summary>
    /// <param name="data"></param>
    private void FreshGoods(GoodsData data)
    {
        // goodsRoot.SetActive(data.Type == GoodType.GoodsChip);
        currentGoodsData = data;
        // goodsImage.sprite = data.Icon;
        // goodsImage.SetNativeSize();
        // goodsNameText.text = LocalizationManager.Instance.GetTextByTag(data.NameTag);
        // FreshTopGoods();
    }

    /// <summary>
    /// 收取物品
    /// </summary>
    /// <param name="times"></param>
    private void ClaimGoods(int times = 1)
    {
        if (!claimAdImage.gameObject.activeSelf || hasWatchedAD)
        {
            switch (currentGoodsData.Type)
            {
                case GoodType.Money: //美元
                    Game.Instance.CurrencyModel.RewardMoney(
                        (ConstantConfig.Instance.GetNumberFromList(8) - Game.Instance.CurrencyModel.GetCurrentMoney()) *
                        this.rateAmount);

                    break;

                default:
                    // GoodsConfig.Instance.AddGoods(currentGoodsData, times);
                    break;
            }
        }

        downCallback?.Invoke();
        downCallback = null;
        Deactivate();
    }
}
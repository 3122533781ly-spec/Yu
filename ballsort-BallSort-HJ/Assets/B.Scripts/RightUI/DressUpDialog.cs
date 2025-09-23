using _02.Scripts.InGame;
using ProjectSpace.Lei31Utils.Scripts.IAPModule;
using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;
using System.Collections.Generic;
using ProjectSpace.BubbleMatch.Scripts.Util;
using UnityEngine;
using UnityEngine.UI;
using ProjectSpace.Lei31Utils.Scripts.Framework.App;
using _02.Scripts.Config;
using System;
using _02.Scripts.Common;
using Unity.Burst.Intrinsics;

public class DressUpDialog : Dialog
{
    [SerializeField] protected Button closeBtn;

    [SerializeField] protected Button buySkinBtn;

    [SerializeField] protected Button adsBtn;
    [SerializeField] private IntNumberDisplayer textAnime;
    [SerializeField] private Transform Tarpos;
    private Button entryBtn;

    public Text priceCoinText;

    public Text adsGetCoinText;

    public Image maskBG;

    public BallView ballView;

    public TubeView tubeView;

    public ThemeView themeView;

    public static GameObject nowTopName;

    public GameObject nowGetAllTip;

    // public Text coinText;//金币数量显示文本

    private bool isInit;

    [SerializeField] public string Tag_Localized;

    private System.Object[] Param { get; set; } = new object[] { };

    // public void SetCoinValueText(int value)
    // {
    //     coinText.text = value.ToString();
    // }

    public override void ShowDialog()
    {
        base.ShowDialog();
        UIEvents.TriggerDressUpDialogOpened();
        // SetCoinValueText(Game.Instance.CurrencyModel.CoinNum);
        RefreshCoinText(-1, default);
        if (ballView.gameObject.activeInHierarchy)
        {
            ballView.StartCoroutine(ballView.PopCloseSlide());
        }
        else if (tubeView.gameObject.activeInHierarchy)
        {
            tubeView.StartCoroutine(tubeView.PopCloseSlide());
        }
        else if (themeView.gameObject.activeInHierarchy)
        {
            themeView.StartCoroutine(themeView.PopCloseSlide());
        }
    }

    public void Init()
    {
        if (isInit)
            return;
        base.Activate(false);
        ResetGameToolByServer();
        //RewardClaimHandle.ClaimReward(new RewardData(GoodType.Coin, 100000), "TestAllSkin", IapStatus.Free);
        ballView.Init();
        tubeView.Init();
        themeView.Init();
        adsGetCoinText.text = ConstantConfig.Instance.GetShopAdsCoin().ToString();
        isInit = true;
        base.Deactivate(false);
    }

    public void ResetGameToolByServer()
    {
        var progress = ServerLogic.Instance.GetProgress<GameSkinProgress>(SoyProfileConst.GameSkinProgress);

        if (progress == null || progress.IsEmpty())
        {
            return;
        }

        for (int i = 0; i < progress.ballValue.Count; i++)
        {
            PlayerPrefs.SetInt("BallSkin" + progress.ballValue[i], 1);
        }

        for (int i = 0; i < progress.tubeValue.Count; i++)
        {
            PlayerPrefs.SetInt("TubeSkin" + progress.tubeValue[i], 1);
        }

        for (int i = 0; i < progress.themeValue.Count; i++)
        {
            PlayerPrefs.SetInt("ThemeSkin" + progress.themeValue[i], 1);
        }

        PlayerPrefs.SetInt("ClickBallSkin", progress.useBallId);

        PlayerPrefs.SetInt("ClickTubeSkin", progress.useTubeId);

        PlayerPrefs.SetInt("ClickThemeSkin", progress.useThemeId);

        print("读取皮肤完毕");
    }

    /// <summary>
    /// 获取当前进度，存于服务器
    /// </summary>
    public void SetGameSkinServerProgress()
    {
        List<int> unlockBall = new List<int>();
        for (int i = 0; i < ballView.unlockBall.Count; i++)
        {
            unlockBall.Add(ballView.unlockBall[i].id);
        }

        List<int> unlockTube = new List<int>();
        for (int i = 0; i < tubeView.unlockTube.Count; i++)
        {
            unlockTube.Add(tubeView.unlockTube[i].id);
        }

        List<int> unlockTheme = new List<int>();
        for (int i = 0; i < themeView.unlockTheme.Count; i++)
        {
            unlockTheme.Add(themeView.unlockTheme[i].id);
        }

        var serialObj = new GameSkinProgress(unlockBall, unlockTube, unlockTheme, PlayerPrefs.GetInt("ClickBallSkin"),
            PlayerPrefs.GetInt("ClickTubeSkin"), PlayerPrefs.GetInt("ClickThemeSkin"));

        ServerLogic.Instance.SetServerProgress(SoyProfileConst.GameSkinProgress, serialObj, true);

        print("存储皮肤完毕");
    }

    private void OnEnable()
    {
        // maskBG.sprite = InGameManager.Instance.bg.sprite;
        closeBtn.onClick.AddListener(CloseMenuBtn);
        buySkinBtn.onClick.AddListener(ClickBuySkinBty);
        adsBtn.onClick.AddListener(WachAdsGetCoin);
        Game.Instance.CurrencyModel.RegisterCoinChangeAction(RefreshCoinText);
    }

    private void OnDisable()
    {
        closeBtn.onClick.RemoveListener(CloseMenuBtn);
        buySkinBtn.onClick.RemoveListener(ClickBuySkinBty);
        adsBtn.onClick.RemoveListener(WachAdsGetCoin);
        Game.Instance.CurrencyModel.UnregisterCoinChangeAction(RefreshCoinText);
    }

    private void CloseMenuBtn()
    {
        DialogManager.Instance.GetDialog<DressUpDialog>().CloseDialog();
    }

    public void SetBtnState(Button btnObj, bool isOn)
    {
        btnObj.SetActive(isOn);
        entryBtn = btnObj;
    }

    public RewardData CheckLockSkin()
    {
        RewardData _skinData;
        List<GoodType> tempSkinType = new List<GoodType>();
        if (ballView.lockBall.Count > 0)
        {
            tempSkinType.Add(GoodType.SkinBall);
        }
        else
        {
            print("球皮肤解锁完毕");
        }
        if (themeView.lockTheme.Count > 0)
        {
            tempSkinType.Add(GoodType.SkinTheme);
        }
        else
        {
            print("背景皮肤解锁完毕");
        }

        int value = UnityEngine.Random.Range(0, tempSkinType.Count);
        int skinValue;

        skinValue = ClickLockSkinData(tempSkinType[value]);

        return _skinData = new RewardData(tempSkinType[value], skinValue);
    }

    public void RewardGetSkin(GoodType skinType, int id, bool isForce = false)
    {
        print("解锁" + skinType + "ID:" + id);
        ShowGetSkinUI(skinType, id, isForce: isForce);
        switch (skinType)
        {
            case GoodType.SkinBall:
                ballView.GetSkin(id);
                break;

            case GoodType.SkinTheme:
                themeView.GetSkin(id);
                break;

            default:
                break;
        }
    }

    private Sprite tempSprite;

    public void ShowGetSkinUI(GoodType skinType, int skinId, int subtype = 0, bool isForce = false)
    {
        DialogManager.Instance.GetDialog<GetNewSkinDialog>().ShowDialog();
        switch (skinType)
        {
            case GoodType.SkinBall:

                var ballObj = ballView.lockBall.Find(b => b.id == skinId);
                if (isForce && ballObj == null)
                {
                    ballObj = ballView.unlockBall.Find(b => b.id == skinId);
                }

                tempSprite = ballObj.icon.sprite;
                break;

            case GoodType.SkinTube:

                var tubeObj = tubeView.lockTube.Find(b => b.id == skinId);
                if (isForce && tubeObj == null)
                {
                    tubeObj = tubeView.unlockTube.Find(b => b.id == skinId);
                }

                tempSprite = tubeObj.icon.sprite;
                break;

            case GoodType.SkinTheme:

                var themeObj = themeView.lockTheme.Find(b => b.id == skinId);
                if (isForce && themeObj == null)
                {
                    themeObj = themeView.unlockTheme.Find(b => b.id == skinId);
                }

                if (themeObj != null)
                    tempSprite = themeObj.icon.sprite;
                break;

            case GoodType.Tool:
            case GoodType.Coin:
                tempSprite = SpriteManager.Instance.GetGoodTypeIcon(skinType, subtype);
                break;

            default:
                tempSprite = null;
                print("未找到Icon");
                break;
        }

        DialogManager.Instance.GetDialog<GetNewSkinDialog>().Init(skinType, skinId, tempSprite, subtype);
    }

    //随机返回一个皮肤ID
    public int ClickLockSkinData(GoodType type)
    {
        switch (type)
        {
            case GoodType.SkinBall:
                return ballView.GetRdmSkinData().id;

            //case GoodType.SkinTube:
            //    return tubeView.GetRdmSkinData().id;

            case GoodType.SkinTheme:
                return themeView.GetRdmSkinData().id;

            default:
                return -1;
        }
    }

    public void UseSkin(GoodType type, int skinId)
    {
        switch (type)
        {
            case GoodType.SkinBall:
                ballView.unlockBall.Find(b => b.id == skinId).toggle.isOn = true;
                break;

            case GoodType.SkinTube:
                tubeView.unlockTube.Find(b => b.id == skinId).toggle.isOn = true;
                break;

            case GoodType.SkinTheme:
                themeView.unlockTheme.Find(b => b.id == skinId).toggle.isOn = true;
                break;
        }
    }

    public void WachAdsGetCoin()
    {
        ADMudule.ShowRewardedAd("DressUpDialog", (isSuccess) =>
        {
            if (isSuccess)
            {
                CoinFlyAnim.Instance.DressBtnPlay(Tarpos, ConstantConfig.Instance.GetShopAdsCoin(), adsBtn.transform.position,
                    AnimIconType.Coin,
                    () =>
                    {
                        RewardClaimHandle.ClaimReward(new RewardData(GoodType.Coin, ConstantConfig.Instance.GetShopAdsCoin()),
                            "GetShopAdsCoin", IapStatus.Free);
                    });

                // SetCoinValueText(Game.Instance.CurrencyModel.CoinNum);
            }
        });
    }

    //返回多语言字符
    private string SetLocalizedText()
    {
        if (!string.IsNullOrEmpty(Tag_Localized))
        {
            return "There are not enough gold coins";
        }

        return "";
    }

    public void ClickBuySkinBty()
    {
        if (Game.Instance.CurrencyModel.CoinNum < int.Parse(priceCoinText.text))
        {
            FloatingWindow.Instance.Show(SetLocalizedText());
            return;
        }

        Game.Instance.CurrencyModel.ConsumeCoin(int.Parse(priceCoinText.text));
        // SetCoinValueText(Game.Instance.CurrencyModel.CoinNum);
        if (ballView.gameObject.activeInHierarchy)
        {
            ballView.GetRdmSkin();
        }
        else if (themeView.gameObject.activeInHierarchy)
        {
            themeView.GetRdmSkin();
        }
        DialogManager.Instance.GetDialog<DressUpDialog>().SetGameSkinServerProgress();
    }

    public override void CloseDialog()
    {
        base.CloseDialog();
        UIEvents.TriggerDressUpDialogClosed();
        if (entryBtn)
            entryBtn.gameObject.SetActive(true);

        SetGameSkinServerProgress();
    }

    public void ShowGetAllTip(GameObject tip)
    {
        if (nowGetAllTip != null)
        {
            nowGetAllTip.SetActive(false);
        }

        nowGetAllTip = tip;
        nowGetAllTip.SetActive(true);
    }

    private void RefreshCoinText(int a, int b)
    {
        var num = Game.Instance.CurrencyModel.CoinNum;
        if (a == -1)
        {
            textAnime.ResetNumber(num);
        }
        else
        {
            textAnime.Number = num;
        }
    }
}
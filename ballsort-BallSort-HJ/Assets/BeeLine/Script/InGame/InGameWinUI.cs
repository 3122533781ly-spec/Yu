using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class InGameWinUI : ElementUI<InGameLineBee>
{
    public void Show()
    {
        //if (this == null)
        //{
        //    return;
        //}
        Activate();
        _groupContent.alpha = 0f;
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.15f);
        seq.Append(_groupContent.DOFade(1f, 0.3f));
    }

    private void WatchADTripleCoin()
    {
        ADMudule.ShowRewardedAd(ADPosConst.WinTriple, (isSuccess) =>
        {
            if (isSuccess)
            {
                Game.Instance.Model.WatchTripleCoinTime.Value++;
                LineBeeInGameAudio.Instance.PlayRewardGet();

                //   Game.Instance.CurrencyModel.RewardCoin(GameConfig.Instance.LevelFixedRewardCoin * 2);
                //   IStaticDelegate.SourceCurrency("Coin", GameConfig.Instance.LevelFixedRewardCoin * 2, "AD", "ADTriple");

                //有可能回调到来之时 UI 已经被销毁
                if (this != null)
                {
                    PlayCoinTripleAnim();
                }

                //_btnTripleCoin.interactable = false;
            }
        });
    }

    private void PlayCoinTripleAnim()
    {
        // Vector2 form = UICamera.Instance.Camera.WorldToScreenPoint(_tripleAreaRect.transform.position);
        //   Vector2 to = UICamera.Instance.Camera.WorldToScreenPoint(_coinIconRect.position);
        //CoinFlyAnim.Instance.Play(5, form, to, () =>
        //{
        //    // int reward = GameConfig.Instance.LevelFixedRewardCoin * 3;
        //    //    _textRewardCoin.DOText(reward.ToString(), 0.2f);
        //});
    }

    private void OnEnable()
    {
        _btnClaim.onClick.AddListener(ReceiveReward);
        _btnAdClaim.onClick.AddListener(ReceiveAdReward);
        // _textRewardCoin.text = GameConfig.Instance.LevelFixedRewardCoin.ToString();
        // _btnNextLevel.onClick.AddListener(ClickNextLevel);
        // _btnTripleCoin.onClick.AddListener(WatchADTripleCoin);
        //_btnBackHome.onClick.AddListener(ClickBackHome);
    }

    private void OnDisable()
    {
        _btnClaim.onClick.RemoveListener(ReceiveReward);
        _btnAdClaim.onClick.RemoveListener(ReceiveAdReward);
        // _btnNextLevel.onClick.RemoveListener(ClickNextLevel);
        //_btnTripleCoin.onClick.RemoveListener(WatchADTripleCoin);
        // _btnBackHome.onClick.RemoveListener(ClickBackHome);
    }

    private void ClickBackHome()
    {
        //   LineBee.Instance.BackHome();
    }

    private void ReceiveReward()
    {
        Game.Instance.CurrencyModel.RewardCoin(5);
        Game.Instance.CurrencyModel.RewadDiamond(1);
        LineBee.Instance.HomeEnterHandle();
        LineBee.Instance.EnterGame();
    }

    private void ReceiveAdReward()
    {
        ADMudule.ShowRewardedAd(ADPosConst.WinTriple, (isSuccess) =>
        {
            if (isSuccess)
            {
                Game.Instance.Model.WatchTripleCoinTime.Value++;
                InGameAudio.Instance.PlayRewardGet();
                Game.Instance.CurrencyModel.RewardCoin(10);
                Game.Instance.CurrencyModel.RewadDiamond(1);
                LineBee.Instance.HomeEnterHandle();
                LineBee.Instance.EnterGame();
            }
        });
    }

    private void ClickNextLevel()
    {
        if (LineBee.Instance.PowerSystem.CanConsume())
        {
            LineBee.Instance.HomeEnterHandle();
            LineBee.Instance.EnterGame();

            //if (LineBee.Instance.GetSystem<AdStrategySystem>().CanShowInterstitial())
            //{
            //    ADMudule.ShowInterstitialAds(ADPosConst.NextLevel, (o) => { });
            //}
        }
        else
        {
            if (LineBee.Instance.LevelModel.MaxUnlockLevel.Value <= 10)
            {
                // DialogManager.Instance.GetDialog<GamePowerDialog>().Activate();
            }
            else
            {
                //DialogManager.Instance.GetDialog<GamePowerVIPDialog>().Activate();
            }
        }
    }

    [SerializeField] private Button _btnClaim;
    [SerializeField] private Button _btnAdClaim;
    [SerializeField] private CanvasGroup _groupContent;
}
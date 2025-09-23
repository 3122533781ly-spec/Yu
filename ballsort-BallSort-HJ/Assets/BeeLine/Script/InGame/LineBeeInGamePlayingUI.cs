using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using ProjectSpace.BubbleMatch.Scripts.Util;
using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;
using Spine;
using UnityEngine;
using UnityEngine.UI;

public class LineBeeInGamePlayingUI : ElementUI<InGameLineBee>
{
    public void GotoBallSort()
    {
        if (Game.Instance.CurrencyModel.DiamondNum < 1)
        {
            DialogManager.Instance.GetDialog<DiamondGetDialog>().ShowDialog();
            return;
        }
        Game.Instance.CurrencyModel.ConsumeDiamond(1);
        TransitionManager.Instance.Transition(0.5f, () => { SceneManager.LoadScene("InGameScenario"); },
                     0.5f);
    }

    private void ClickSetting()
    {
        DialogManager.Instance.GetDialog<OptionDialog>().ShowDialog();
    }

    public void OpenSkinMainUI()
    {
        DialogManager.Instance.GetDialog<DressUpDialog>().ShowDialog();
        _balls.SetActive(false);
    }

    //[SerializeField] public CanvasGroup TopGroup;

    //public async Task PlayEnterAnim()
    //{
    //    float animTime = 0.3f;
    //    TopGroup.DOFade(1f, animTime);
    //    _btnGetCoin.GetComponent<RectTransform>().DOScale(1f, animTime);
    //    _btnUseHint.GetComponent<RectTransform>().DOScale(1f, animTime);
    //    _btnSkipLevel.GetComponent<RectTransform>().DOScale(1f, animTime);
    //    _btnSetting.GetComponent<RectTransform>().DOScale(1f, animTime);
    //    await TaskExtension.DelaySecond(animTime);
    //}

    //private void AnimReady()
    //{
    //    TopGroup.alpha = 0;
    //    _btnGetCoin.GetComponent<RectTransform>().localScale = Vector3.zero;
    //    _btnUseHint.GetComponent<RectTransform>().localScale = Vector3.zero;
    //    _btnSkipLevel.GetComponent<RectTransform>().localScale = Vector3.zero;
    //    _btnSetting.GetComponent<RectTransform>().localScale = Vector3.zero;
    //}

    private void OnEnable()
    {
        _gotoBallSort.onClick.AddListener(GotoBallSort);
        skinEntryBtn.onClick.AddListener(OpenSkinMainUI);
        btnSetting.onClick.AddListener(ClickSetting);
        //_isSettingPanelShow = false;
        //_settingPanelRect.SetAnchoredPositionY(_hideSettingPanelY);
        Refresh();
        //_btnSkipLevel.onClick.AddListener(ClickSkipLevel);
        _btnUseHint.onClick.AddListener(ClickHint);
        _btnGetCoin.onClick.AddListener(ClickGetCoin);
        _btnGetPower.onClick.AddListener(GetPower);
        Playbtn.onClick.AddListener(EnterGame);
        //_btnExit.onClick.AddListener(ClickExit);
        //_btnSetting.onClick.AddListener(ClickSetting);
        //_btnRestart.onClick.AddListener(ClickRestart);
        UIEvents.OnDressUpDialogOpened += OnDressUpDialogOpened;
        UIEvents.OnDressUpDialogClosed += OnDressUpDialogClosed;
    }

    private void OnDisable()
    {
        //    _btnSkipLevel.onClick.RemoveListener(ClickSkipLevel);
        _btnUseHint.onClick.RemoveListener(ClickHint);
        _btnGetCoin.onClick.RemoveListener(ClickGetCoin);
        _btnGetPower.onClick.RemoveListener(GetPower);
        //    _btnExit.onClick.RemoveListener(ClickExit);
        //    _btnSetting.onClick.RemoveListener(ClickSetting);
        //    _btnRestart.onClick.RemoveListener(ClickRestart);
        UIEvents.OnDressUpDialogOpened -= OnDressUpDialogOpened;
        UIEvents.OnDressUpDialogClosed -= OnDressUpDialogClosed;
    }

    //private void ClickRestart()
    //{
    //    App.Instance.EnterGame();
    //}
    private void OnDressUpDialogOpened()
    {
        // 皮肤UI打开时，隐藏_balls
        if (_balls != null)
        {
            _balls.SetActive(false);
            Debug.Log("_balls 已隐藏");
        }
    }

    private void OnDressUpDialogClosed()
    {
        // 皮肤UI关闭时，显示_balls
        if (_balls != null && _context.StateModel.CurrentState == InLineBeeGameState.Playing)
        {
            _line.gameObject.GetComponent<LineRenderer>().startColor = SpriteManager.Instance.GetLineColor();
            _line.gameObject.GetComponent<LineRenderer>().endColor = SpriteManager.Instance.GetLineColor();
            _balls.SetActive(true);
            Debug.Log("_balls 已显示");
        }
    }

    private void ClickHint()
    {
        if (LineBee.Instance.LevelModel.HasHint)
        {
            Context.GetController<LineBeeInGameHintControl>().ShowHint();
            return;
        }
        if (Game.Instance.CurrencyModel.CoinNum < 20)
        {
            DialogManager.Instance.GetDialog<CoinGetDialog>().ShowDialog();
            return;
        }
        Game.Instance.CurrencyModel.ConsumeCoin(20);
        Context.GetController<LineBeeInGameHintControl>().ShowHint();
        LineBee.Instance.LevelModel.SetHasHint(true);
        //}
        //else
        //{
        //    if (App.Instance.CurrencyModel.CoinNum >= GameConfig.Instance.HintConsumeCoin)
        //    {
        //        App.Instance.CurrencyModel.ConsumeCoin(GameConfig.Instance.HintConsumeCoin);
        //        RewardClaimHandle.ConsumeCoin(GameConfig.Instance.HintConsumeCoin, "System", "Hint");
        //        Context.LevelModel.SetHasHint();
        //        _hintCoinPanel.SetActive(false);
        //        Context.GetController<InGameHintControl>().ShowHint();
        //    }
        //    else
        //    {
        //        FloatingWindow.Instance.Show("You not have enough coin.");
        //        DialogManager.Instance.GetDialog<CoinGetDialog>().Activate();
        //    }
        //}
    }

    //private void ClickSetting()
    //{
    //    if (_isSettingPanelShow)
    //    {
    //        HideSettingPanel();
    //    }
    //    else
    //    {
    //        ShowSettingPanel();
    //    }
    //}

    //private void ClickExit()
    //{
    //    App.Instance.BackHome();
    //}
    public void GetPower()
    {
        DialogManager.Instance.GetDialog<PowerGetDialog>().ShowDialog();
    }

    private void EnterGame()
    {
        if (LineBee.Instance.PowerSystem.GamePower.Value < 5)
        {
            DialogManager.Instance.GetDialog<PowerGetDialog>().ShowDialog();
            return;
        }
        _context.StateModel.CurrentState = InLineBeeGameState.Playing;
        LineBee.Instance.PowerSystem.ConsumeGamePower();
        LineBee.Instance.LevelModel.SetHasHint(false);
        foreach (var stage in gameStages)
        {
            if (stage != null) // 额外检查物体是否存在
            {
                stage.SetActive(true);
                var sequence = DOTween.Sequence();

                sequence.Append(stage.transform.DOScale(1.15f, 0.295f));
                sequence.Append(stage.transform.DOScale(0.85f, 0.17f));
                sequence.Append(stage.transform.DOScale(1.00f, 0.17f));

                sequence.SetEase(Ease.OutQuart);
                sequence.SetAutoKill(true);
            }
            Playbtn.SetActive(false);
            _circle.SetActive(false);
        }
    }

    private void ClickGetCoin()
    {
        ADMudule.ShowRewardedAd(ADPosConst.GetCoinDialog, (isSuccess) =>
        {
            if (isSuccess)
            {
                //IStaticDelegate.SourceCurrency("Coin", GameConfig.Instance.AdRewardCoin, "AD",
                //    ADPosConst.GetCoinInGame);
                Game.Instance.CurrencyModel.RewardCoin(20);
            }
        });
    }

    //private void ClickSkipLevel()
    //{
    //    ADMudule.ShowRewardedAd(ADPosConst.SkipLevel, (isSuccess) =>
    //    {
    //        if (isSuccess)
    //        {
    //            App.Instance.LevelModel.PassCurrentLevel();
    //            App.Instance.EnterGame();
    //        }
    //    });
    //}

    private void Refresh()
    {
        _textLevel.text = $"{LineBee.Instance.LevelModel.MaxUnlockLevel.Value} LEVEL";
        _line.gameObject.GetComponent<LineRenderer>().startColor = SpriteManager.Instance.GetLineColor();
        _line.gameObject.GetComponent<LineRenderer>().endColor = SpriteManager.Instance.GetLineColor();
        //_textGetCoinCount.text = $"+{GameConfig.Instance.AdRewardCoin}";
        //_textHintCoin.text = GameConfig.Instance.HintConsumeCoin.ToString();
    }

    //private void HideSettingPanel()
    //{
    //    DOTween.Kill(_settingPanelRect.gameObject);
    //    _settingPanelRect.DOAnchorPosY(_hideSettingPanelY, 0.3f);
    //    _isSettingPanelShow = false;
    //}

    //private void ShowSettingPanel()
    //{
    //    DOTween.Kill(_settingPanelRect.gameObject);
    //    _settingPanelRect.DOAnchorPosY(_displaySettingPanelY, 0.3f);
    //    _isSettingPanelShow = true;
    //}

    //[SerializeField] private Text _textLevel;
    //[SerializeField] private Text _textGetCoinCount;
    //[SerializeField] private Text _textHintCoin;
    [SerializeField] private Button _gotoBallSort;

    [SerializeField] private Button skinEntryBtn;
    [SerializeField] private Button btnSetting;
    [SerializeField] private Button Playbtn;
    [SerializeField] private Button _btnGetCoin;
    [SerializeField] private Button _btnUseHint;
    [SerializeField] private Button _btnGetPower;
    [SerializeField] private Text _textLevel;
    [SerializeField] private List<GameObject> gameStages = new List<GameObject>();
    [SerializeField] private GameObject _playbtn;
    [SerializeField] private GameObject _balls;
    [SerializeField] private GameObject _line;
    [SerializeField] private GameObject _circle;
    //[SerializeField] private Button _btnExit;
    //[SerializeField] private Button _btnSetting;
    //[SerializeField] private Button _btnRestart;

    //[SerializeField] private RectTransform _settingPanelRect;
    //[SerializeField] private float _displaySettingPanelY;
    //[SerializeField] private float _hideSettingPanelY;
    //[SerializeField] private GameObject _hintCoinPanel;

    //private bool _isSettingPanelShow;
}
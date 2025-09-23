using _02.Scripts.InGame;
using _02.Scripts.InGame.Controller;
using _02.Scripts.InGame.State;
using _02.Scripts.InGame.UI;
using Fangtang;
using ProjectSpace.BubbleMatch.Scripts.Util;
using ProjectSpace.Lei31Utils.Scripts.Framework.App;
using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;
using ProjectSpace.WangZ.Scripts.InGame;
using Sirenix.OdinInspector;
using UnityEngine;

public class InGame : GenericSceneElement<InGame, InGameState>, Prime31.IObjectInspectable
{
    public InGameModel CellMapModel { private set; get; }

    protected override void OnInit(object data)
    {
        ADMudule.ShowBanner();
        CellMapModel = new InGameModel();
        Models.Add(CellMapModel);

        Views.Add(GetComponentInChildren<InGamePlayingUI>(true));
        Views.Add(GetComponentInChildren<InGamePauseUI>(true));
        Views.Add(GetComponentInChildren<InGameSuccessUI>(true));
        Views.Add(GetComponentInChildren<InGameWinReward>(true));
        Views.Add(GetComponentInChildren<InGameCircleItem>(true));
        Views.Add(GetComponentInChildren<InGameBoxDialog>(true));
        Views.Add(GetComponentInChildren<FristInGameReward>(true));

        Controllers.Add(GetComponent<InGameTimeController>());
        Controllers.Add(GetComponentInChildren<InGameGuideController>());
        Controllers.Add(GetComponentInChildren<InGameMapController>());
        Controllers.Add(GetComponentInChildren<InGameMatchController>());

        AddState(new InGameStandbyState(), InGameState.Standby);
        AddState(new InGamePlayingState(), InGameState.Playing);
        AddState(new InGamePauseState(), InGameState.Pause);
        AddState(new InGameWinState(), InGameState.Win);
        AddState(new InGameFailedState(), InGameState.Failed);

        AddStateTransition<InGameStandbyState, InGamePlayingState>();
        AddStateTransition<InGamePlayingState, InGameStandbyState>();
        AddStateTransition<InGamePlayingState, InGamePauseState>();
        AddStateTransition<InGamePlayingState, InGameStandbyState>();
        AddStateTransition<InGamePauseState, InGamePlayingState>();
        AddStateTransition<InGamePlayingState, InGameWinState>();
        AddStateTransition<InGamePlayingState, InGameFailedState>();
        AddStateTransition<InGameWinState, InGameStandbyState>();
        AddStateTransition<InGameWinState, InGamePlayingState>();
        AddStateTransition<InGameFailedState, InGamePlayingState>();
        AddStateTransition<InGameStandbyState, InGamePauseState>();
        AddStateTransition<InGamePauseState, InGameWinState>();
        AddStateTransition<InGameFailedState, InGameStandbyState>();
        AddStateTransition<InGamePauseState, InGameStandbyState>();

        Restart();
        GetView<FristInGameReward>().CheckIsShow();

        Game.Instance.GetSystem<Redeem.RedeemSystem>().InitRedeem();
        CheckGuide();

        // AudioClipHelper.Instance.PlaySound(AudioClipEnum.Seagull);
    }

    private void Start()
    {
        DialogManager.Instance.GetDialog<DressUpDialog>().Init();
        SpriteManager.Instance.InitSkin();
    }

    private void SetPlaying()
    {
        Game.Instance.GetSystem<InGameSystem>().IsPlaying = StateModel.CurrentState == InGameState.Playing;
    }

    public void Restart()
    {
        Debug.Log("Restart");
        Game.Instance.LevelModel.PlusLevelAttemptNum();

        StateModel.CurrentState = InGameState.Standby;
        StateModel.CurrentState = InGameState.Playing;

        SetPlaying();
        InitData();
        GetView<InGamePlayingUI>().RefreshUI();
        GetView<InGamePlayingUI>().ShowSlotItemHideBigTurn(Game.Instance.LevelModel.CopiesType == CopiesType.Thread);
        GetController<InGameGuideController>().CheckGuid();
        Game.Instance.GameStatic.PlusOpenGameTime();
        // JobUtils.Delay(0.5f, StartGame);
    }

    /// <summary>
    /// 10?????????????¦Â???
    /// ???????????§Ö?
    /// ????????§Ö?
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="forceShow"></param>
    public void CheckInterpolationAd(string pos, bool forceShow = false)
    {
        //App.Instance.LevelModel.MaxUnlockLevel.Value >= ConstantConfig.Instance.GetInterpolationAd() ||????????
        if (forceShow)
        {
            ADMudule.ShowInterstitialAds(pos, _ => { Restart(); });
        }
        else
        {
            Restart();
        }
    }

    public void StartGame()
    {
        Debug.Log("StartGame");
        StateModel.CurrentState = InGameState.Playing;
        SetPlaying();
        //JobUtils.Delay(0.2f, () => { GetController<InGameGuideController>().StratGuid(); });
    }

    public void Continue()
    {
        Debug.Log("Continue");
        StateModel.CurrentState = InGameState.Playing;
        SetPlaying();
    }

    public void Pause(bool isPause = true)
    {
        Debug.Log("Pause");
        Game.Instance.GetSystem<InGameSystem>().IsClickPause = isPause;
        StateModel.CurrentState = InGameState.Pause;
        SetPlaying();
    }

    public void Failed()
    {
        Debug.Log("Failed");
        StateModel.CurrentState = InGameState.Failed;
        SetPlaying();
    }

    public void Win()
    {
        Debug.Log("Win");
        StateModel.CurrentState = InGameState.Win;
        SetPlaying();
    }

    public bool IsWin()
    {
        return StateModel.CurrentState == InGameState.Win;
    }

    private void OnDestroy()
    {
        GetController<InGameGuideController>().Dispose();
        GetModel<InGameModel>().Dispose();
    }

    private void InitData()
    {
        GetController<InGameMapController>().StartGame();
    }

    public void CheckGuide()
    {
        GetController<InGameGuideController>().CheckGuid();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Win();
        }
    }

    public void CheckIsOver()
    {
        var levelData = CellMapModel.LevelPipeList;
        var isOver = levelData.Find(x => !x.PipeFullOrEmpty()) == null;
        if (isOver)
        {
            Win();
        }
    }

#if UNITY_EDITOR

    [Button]
    public void JumpToLevel(int a)
    {
        Game.Instance.LevelModel.EnterLevelID = a;
        Game.Instance.LevelModel.MaxUnlockLevel.Value = a;
        SoyProfile.Set(SoyProfileConst.NormalLevel, a);
    }

    [Button]
    public void TestCoin()
    {
        CoinFlyAnim.Instance.Play(10, gameObject.transform.position);
    }

#endif
}

public enum InGameState
{
    Null,
    Standby,
    Playing,
    Pause,
    Win,
    Failed,
}
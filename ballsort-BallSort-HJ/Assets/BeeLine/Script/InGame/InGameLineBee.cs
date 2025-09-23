using Fangtang;
using ProjectSpace.BubbleMatch.Scripts.Util;
using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;
using Sirenix.OdinInspector;
using UnityEngine;

public class InGameLineBee : GenericSceneElement<InGameLineBee, InLineBeeGameState>, Prime31.IObjectInspectable
{
    [SerializeField] public Camera GameCamera = null;

    //public InGameLevelModel LevelModel { get; private set; }
    public InGameMatchModel MatchModel { get; private set; }

    public InGameEventModel EventModel { get; private set; }

    protected override void OnInit(object data)
    {
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(SceneManager.PreviousScene))
        {
            //App.Instance.LevelModel.SetEnterLevelID(App.Instance.LevelModel.MaxUnlockLevel.Value);
        }
#endif
        //LevelModel = new InGameLevelModel();
        MatchModel = new InGameMatchModel();
        EventModel = new InGameEventModel();

        Views.Add(GetComponent<InGamePointEnterAnim>());
        Views.Add(GetComponent<InGameLineView>());
        Views.Add(GetComponentInChildren<LineBeeInGamePlayingUI>(true));
        Views.Add(GetComponentInChildren<InGameFailedUI>(true));
        Views.Add(GetComponentInChildren<InGameWinUI>(true));
        Views.Add(GetComponentInChildren<InGameTutorialUI>(true));

        Controllers.Add(GetComponent<InGameLevelLoader>());
        //Controllers.Add(GetComponent<InGameEventController>());
        Controllers.Add(GetComponent<InGameInputController>());
        Controllers.Add(GetComponent<InGameCameraSizeFit>());
        Controllers.Add(GetComponent<InGameLinkControl>());
        Controllers.Add(GetComponent<LineBeeInGameHintControl>());

        AddState(new LineBeeInGameStandbyState(), InLineBeeGameState.Standby);
        AddState(new LineBeeInGamePlayingState(), InLineBeeGameState.Playing);
        AddState(new LineBeeInGameWinState(), InLineBeeGameState.Win);
        AddState(new LineBeeInGameFailedState(), InLineBeeGameState.Failed);

        AddStateTransition<LineBeeInGameStandbyState, LineBeeInGamePlayingState>();
        AddStateTransition<LineBeeInGamePlayingState, LineBeeInGameWinState>();
        AddStateTransition<LineBeeInGamePlayingState, LineBeeInGameFailedState>();
    }

    private void Start()
    {
        DialogManager.Instance.GetDialog<DressUpDialog>().Init();
        SpriteManager.Instance.InitSkin();
    }

    public void GameWin()
    {
        StateModel.CurrentState = InLineBeeGameState.Win;
    }

    public void GameFailed()
    {
        StateModel.CurrentState = InLineBeeGameState.Failed;
    }
}

public enum InLineBeeGameState
{
    Null,
    Standby,
    Playing,
    Win,
    Failed,
}
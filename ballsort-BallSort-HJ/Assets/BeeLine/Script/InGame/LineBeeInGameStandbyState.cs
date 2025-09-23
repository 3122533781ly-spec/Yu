using Prime31.StateKit;
using UnityEngine;

public class LineBeeInGameStandbyState : SKState<InGameLineBee>
{
    public override void begin()
    {
        //   App.Instance.LevelModel.PlusTodayLevelCount();
        // App.Instance.LevelModel.ResetReviveUseAD();

        LineBeeLevelData levelData = LevelProvider.GetInGameLevelData(LineBee.Instance.LevelModel.MaxUnlockLevel.Value);
        //  _context.LevelModel.SetData(levelData);

        _context.GetController<InGameLevelLoader>().LoadLevel(levelData);

        HandleStatic();
        HandleComponent();

        //        if (App.Instance.GetSystem<ADStrategySystem>().NeedShowBanner())
        //        {
        //            ADMudule.ShowBanner();
        //        }
    }

    private void HandleStatic()
    {
        StaticModule.GameFlow_EnterGame();

        //StaticModule.BeginStage(_context.LevelModel.GetStaticLevelID());
    }

    private void HandleComponent()
    {
        // _context.GetController<InGameEventController>().Deactivate();
        //  _context.GetController<InGameInputController>().Deactivate();
    }
}
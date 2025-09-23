﻿using Prime31.StateKit;
using ProjectSpace.Lei31Utils.Scripts.Framework.App;

public class InGameFailedState : SKState<InGame>
{
    public override void begin()
    {
        HandleStatic();
        HandleComponent();
    }

    private void HandleComponent()
    {
        _context.GetController<InGameTimeController>().Deactivate();
    }

    private void HandleStatic()
    {
        StaticModule.FailedStage(Game.Instance.LevelModel.EnterLevelID, Game.Instance.LevelModel.LevelAttemptNum.Value, Game.Instance.LevelModel.CopiesType);
    }
}
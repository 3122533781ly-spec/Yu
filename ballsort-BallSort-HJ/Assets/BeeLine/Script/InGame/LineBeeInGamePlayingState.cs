using Prime31.StateKit;
using UnityEngine;

public class LineBeeInGamePlayingState : SKState<InGameLineBee>
{
    public override void begin()
    {
        ADMudule.HideBanner();
        HandleComponent();
        _context.GetView<InGamePointEnterAnim>().PlayMoveIn();
        //  _context.GetView<InGamePointEnterAnim>().PlayMoveIn();
    }

    private void HandleComponent()
    {
        //_context.GetController<InGameEventController>().Activate();
        //if (App.Instance.LevelModel.EnterMode == LevelMode.LinkFlip)
        //{
        //    _context.GetController<InGameInputController>().Activate();
        //}
        //else
        //{
        //}

        //        _context.GetController<InGameSlotPositionUpdater>().Activate();
        //        _context.GetController<InGameTimeController>().Activate();
        //        _context.GetView<InGamePlayingUI>().SetClockMove(true);
        //        _context.GetController<InGameMoneyTileController>().Activate();
    }

    public override void end()
    {
    }
}
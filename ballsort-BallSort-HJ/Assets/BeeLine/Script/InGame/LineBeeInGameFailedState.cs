using Prime31.StateKit;

public class LineBeeInGameFailedState : SKState<InGameLineBee>
{
    public override void begin()
    {
        AudioManager.Instance.StopBGM();
        ADMudule.ShowBanner();
        HandleStatic();
        LineBeeInGameAudio.Instance.PlayLoseGame();

        //claim reward
        HandleComponent();

        _context.GetView<InGamePointEnterAnim>().PlayGameFailedAnim(AnimFailedCompleted);
        // LineBee.Instance.EnterGame();
    }

    private void AnimFailedCompleted()
    {
        _context.GetView<InGameFailedUI>().Activate();
    }

    private void HandleStatic()
    {
        // StaticModule.FailedStage(_context.LevelModel.GetStaticLevelID());
    }

    public override void end()
    {
        _context.GetView<InGameFailedUI>().Deactivate();
    }

    private void HandleComponent()
    {
        _context.GetController<InGameInputController>().Deactivate();
        _context.GetController<InGameLinkControl>().Deactivate();
        _context.GetView<InGameLineView>().Deactivate();
    }
}
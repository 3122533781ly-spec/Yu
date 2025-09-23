using Prime31.StateKit;

public class LineBeeInGameWinState : SKState<InGameLineBee>
{
    public override void begin()
    {
        ADMudule.ShowBanner();
        LineBeeInGameAudio.Instance.PlayWinGame();
        AudioManager.Instance.StopBGM();
        HandleStatic();

        //App.Instance.GetSystem<AdStrategySystem>().PassLevel();

        //if (App.Instance.LevelModel.EnterMode == LevelMode.LinkFlip)
        //{
        //    App.Instance.LevelModel.PassCurrentLevel();
        //}
        LineBee.Instance.LevelModel.PassCurrentLevel();
        HandleComponent();
        _context.GetView<InGamePointEnterAnim>().PlayGameWinAnim(AnimEnd);
        //  LineBee.Instance.EnterGame();
    }

    private void AnimEnd()
    {
        //HandleFixedReward();
        _context.GetView<InGameWinUI>().Show();
    }

    //private void HandleFixedReward()
    //{
    //    Game.Instance.CurrencyModel.RewardCoin(30);
    //    // IStaticDelegate.SourceCurrency("Coin", GameConfig.Instance.LevelFixedRewardCoin, "Game", "PassLevel");
    //}

    private void HandleStatic()
    {
        //if (App.Instance.LevelModel.EnterMode == LevelMode.LinkFlip)
        //{
        //    StaticModule.CompletedStage(_context.LevelModel.GetStaticLevelID());
        //    int levelId = App.Instance.LevelModel.EnterLevelID;
        //    StaticModule.SetLevel(levelId);
        //    if (levelId == 20 || levelId == 30 || levelId == 50 || levelId == 70 || levelId == 100)
        //    {
        //        StaticModule.PassLevelState(levelId,
        //            App.Instance.CurrencyModel.CoinNum);
        //    }
        //}
    }

    private void HandleComponent()
    {
        _context.GetController<InGameInputController>().Deactivate();
        _context.GetController<InGameLinkControl>().Deactivate();
        _context.GetView<InGameLineView>().Deactivate();
    }
}
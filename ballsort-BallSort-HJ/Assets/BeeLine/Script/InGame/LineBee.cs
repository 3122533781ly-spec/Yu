using System;
using System.Collections.Generic;
using System.Dynamic;
using Fangtang;
using Fangtang.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

public class LineBee : Singleton<LineBee>
{
    public GameModel Model { get; private set; }
    public LineBeeLevelModel LevelModel { get; private set; }

    //   public AppCurrencyModel CurrencyModel { get; private set; }
    //public SceneManager SceneManager { get; private set; }
    public GamePowerSystem PowerSystem { get; private set; }

    //public SystemGroup SystemGroup { get; private set; }
    //public LocalUserData LocalUser { get; private set; }
    //public GameStatic GameStatic { get; private set; }
    //public AppRemoteConfig RemoteConfig { get; private set; }

    //public bool IsBasicComponentLoaded { get; set; }

    //public void InitCreateInstance()
    //{
    //}

    //public void BackHome()
    //{
    //    TransitionManager.Instance.Transition(0.3f,
    //        () => { SceneManager.LoadScene(SceneManager.HomeScene); },
    //        0.3f, () => { }, true);
    //}

    public void EnterGame()
    {
        // GameStatic.PlusPlayTime();
        //   LevelModel.EnterMode = LevelMode.LinkFlip;
        LevelModel.SetEnterLevelID(LevelModel.MaxUnlockLevel.Value);
        TransitionManager.Instance.Transition(0.3f,
            () => { SceneManager.LoadScene(SceneManager.GameStageScene); },
            0.3f, () => { }, true);
    }

    //public T GetSystem<T>() where T : GameSystem
    //{
    //    //  return SystemGroup.GetSystem<T>();
    //}

    /// <summary>
    /// 从home进入游戏需要的准备工作
    /// </summary>
    public void HomeEnterHandle()
    {
        //ADMudule.HideBanner();
        ////  DialogManager.Instance.ClearAllDialog();
        //GC.Collect();
    }

    private LineBee()
    {
        //int seed = DateTime.Now.Millisecond;
        //LDebug.Log($"App instantiate. seed:{seed}");
        //Random.InitState(seed);
        //// SceneElementManager.Instance.Init();
        //Application.targetFrameRate = 60;
        //SystemGroup = new SystemGroup();
        //Model = new GameModel();
        // CurrencyModel = new AppCurrencyModel();
        PowerSystem = new GamePowerSystem();
        LevelModel = new LineBeeLevelModel();
        //SceneManager = new SceneManager(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        //LocalUser = new LocalUserData();
        //GameStatic = new GameStatic();
        //RemoteConfig = new AppRemoteConfig();
        //InitSystem();
    }

    private void InitSystem()
    {
        //SystemGroup.RegisterSystem(new LocalUserSystem());
        //SystemGroup.RegisterSystem(new RatingSystem());
        //SystemGroup.RegisterSystem(new RemoteControlSystem());
        //  SystemGroup.RegisterSystem(new PropsSystem());
        // SystemGroup.RegisterSystem(new GamePowerSystem());
        //  SystemGroup.RegisterSystem(new AdStrategySystem());
    }

    private void UnloadSystem()
    {
        //SystemGroup.UnregisterSystem<LocalUserSystem>();
        //SystemGroup.UnregisterSystem<RatingSystem>();
        //SystemGroup.UnregisterSystem<RemoteControlSystem>();
        //  SystemGroup.UnregisterSystem<PropsSystem>();
        // SystemGroup.UnregisterSystem<GamePowerSystem>();
        //SystemGroup.UnregisterSystem<AdStrategySystem>();
    }

    protected override void OnDispose()
    {
        //base.OnDispose();
        //UnloadSystem();
        //LDebug.Log("App destory.");
    }
}
using System;

using UnityEngine;



using WeChatWASM;

using SystemInfo = WeChatWASM.SystemInfo;



public static class WXModule

{

    public static bool HasLoginSDK { get; private set; }


    public static string Code { get; private set; }


    public static void SDKLoginCompleted(string code)

    {

        Code = code;

        HasLoginSDK = true;



    }



    public static bool IsInit => SystemInfo != null;


    //缓存的系统信息

    public static SystemInfo SystemInfo { get; private set; }


    /// <summary>

    /// 获取到系统信息，游戏初始场景调用

    /// </summary>

    /// <param name="info"></param>

    public static void FetchSystemInfo(SystemInfo info)

    {

        SystemInfo = info;

        ShowSystemScreenInfo(info);

    }


    private static void ShowSystemScreenInfo(SystemInfo info)

    {

        Debug.Log($"with:{info.screenWidth} height:{info.screenHeight} top-{info.safeArea.top}" +

                  $"bottom-{info.safeArea.bottom} left-{info.safeArea.left} right-{info.safeArea.right}");

    }


    public static void RegisterOnHide(Action onHide)

    {

        _onHide += onHide;

    }


    public static void unregisterOnHide(Action onHide)

    {

        _onHide -= onHide;

    }


    public static void RegisterOnShow(Action onHide)

    {

        _onShow += onHide;

    }


    public static void unregisterOnShow(Action onHide)

    {

        _onShow -= onHide;

    }



    public static void OnHideTrigger(GeneralCallbackResult result)

    {

        _onHide.Invoke();

    }


    public static void OnShowTrigger(OnShowListenerResult result)

    {

        _onShow.Invoke();   

    }



    private static Action _onHide = delegate { };

    private static Action _onShow = delegate { };


    static WXModule()

    {

        HasLoginSDK = false;

    }

}

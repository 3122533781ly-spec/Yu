using Fangtang;

//using SoyBean.Dialog;
using UnityEngine;

public class Home : GenericSceneElement<Home, HomeState>
{
    protected override void OnInit(object data)
    {
        // Views.Add(GetComponentInChildren<HomeMainPage>(true));
        //Views.Add(GetComponent<HomeAnim>());

        StaticModule.GameFlow_EnterHome();
        ADMudule.ShowBanner();
    }
}

public enum HomeState
{
    Null,
    Standby,
}
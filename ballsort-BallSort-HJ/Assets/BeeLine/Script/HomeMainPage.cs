using System.Collections;
using System.Collections.Generic;
using _02.Scripts.Config;
using _02.Scripts.DressUpUI;
using ProjectSpace.Lei31Utils.Scripts.IAPModule;
using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;
using UnityEngine;
using UnityEngine.UI;

public class HomeMainPage : ElementUI<Home>
{
    private void OnEnable()
    {
        _btnPlay.onClick.AddListener(ClickPlay);
        _btnAddPower.onClick.AddListener(ClickAddPower);
        _btnVipLevel.onClick.AddListener(ClickVipLevelUp);
        _btnAddCoin.onClick.AddListener(ClickAddCoin);
    }

    private void OnDisable()
    {
        _btnPlay.onClick.RemoveListener(ClickPlay);
        _btnAddPower.onClick.RemoveListener(ClickAddPower);
        _btnVipLevel.onClick.RemoveListener(ClickVipLevelUp);
        _btnAddCoin.onClick.RemoveListener(ClickAddCoin);
    }

    private void ClickAddCoin()
    {
        //  DialogManager.Instance.GetDialog<CoinGetDialog>().Activate();
    }

    private void ClickVipLevelUp()
    {
        //DialogManager.Instance.GetDialog<GamePowerVIPDialog>().Activate();
    }

    private void ClickAddPower()
    {
        //DialogManager.Instance.GetDialog<GamePowerDialog>().Activate();
    }

    private void ClickPlay()
    {
        //if (App.Instance.GetSystem<GamePowerSystem>().CanConsume())
        //{
        //    AsyncPlay();
        //}
        //else
        //{
        //  //  DialogManager.Instance.GetDialog<GamePowerDialog>().Activate();
        //}
    }

    private async void AsyncPlay()
    {
        //CoinFlyAnim.Instance.SetMaskActive(true);

        await Context.GetView<HomeAnim>().PlayEnterAnim();

        //CoinFlyAnim.Instance.SetMaskActive(false);
        //App.Instance.GetSystem<GamePowerSystem>().ConsumeGamePower();
        //App.Instance.HomeEnterHandle();
        //App.Instance.EnterGame();
    }

    [SerializeField] private Button _btnPlay;
    [SerializeField] private Button _btnAddPower;
    [SerializeField] private Button _btnAddCoin;
    [SerializeField] private Button _btnVipLevel;
}
using System;
using System.Diagnostics;


public class MAXADDelegate : IAD
{
    public void ShowBanner()
    {
    }

    public void HideBanner()
    {
    }

    public bool IsBannerShowing()
    {
        return false;
    }

    public void ShowInterstitialAds(string pos, Action<bool> callBack = null)
    {
        UnityEngine.Debug.Log("插屏广告接口已经调用");
        //GHGYSDK.Instance.ShowInterstitial(pos, () =>
        //{
        //    //广告展示完成，关闭
        //    callBack?.Invoke(true);
        //}, () =>
        //{
        //    //展示失败
        //    callBack?.Invoke(false);
        //});
    }

    public void ShowRewardedAd(string pos, Action<bool> callBack = null)
    {
        //GHGYSDK.Instance.ShowRewardVideo(pos, () =>
        //{
        //    //给用户奖励，此时广告也关闭
        //    callBack?.Invoke(true);
        //}, () =>
        //{
        //    //展示失败
        //    callBack?.Invoke(false);
        //});
    }

    public bool IsInterstitialReady()
    {
        return false;
    }

    public bool IsRewardedAdReady()
    {
        return false;
    }

    public void ShowMRec()
    {
    }

    public void HideMRec()
    {
    }

    public bool IsMRecShowing()
    {
        return false;
    }
}
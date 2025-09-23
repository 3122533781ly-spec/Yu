using System;
using System.Collections.Generic;
using UnityEngine;
using WeChatWASM;
using Fangtang.Utils;
using Newtonsoft.Json;
using UnityEngine.Scripting;
using UnityEngine.UI;
public enum WXReportSceneType
{
    WXInitSuccess = 1001,//微信SDK初始化成功
    LoginBegin,//开始登陆
    LoginSuccess,//登陆成功
    PlayerDataGetSuccess,//玩家存档解析完成
    BuildingSceneBegin,//开始加载建造场景
    BuildingSceneSuccess,//建造场景加载完成
    CheckPlayerOP,//检查到玩家操作
}
[System.Serializable]
public class OpenDataMessage
{
    // type 用于表明时间类型
    public string type;

    public string shareTicket;

    public int score;
}
[Preserve]
public class WXSDKManager : Singleton<WXSDKManager>
{
    String bannerAdUnitId = "adunit-d8cb25811e439613";
    String interstitalAdUnitId = "adunit-28dabfd329d6c6d8";
    String rewardedAdUnitId = "adunit-f099159b763f9b1d";
    WXBannerAd bannerAd;
    WXInterstitialAd interstitialAd;
    WXRewardedVideoAd rewardedVideoAd;

    Action<Boolean, String> loginCallback;

    Boolean vibrating = false;

    private bool reportSceneFinish = false;

    public Action<bool> createButtonAction;
    
    private WXOpenDataContext openDataContext;

    private RawImage RankBody;
    private Canvas RankingCanvas;
    private GameObject RankMask;

    private bool intestitalReady = false;
    private Action<bool> shareCallback;
    private DateTime shareTime;
    
    [Preserve]
    WXSDKManager()
    {

    }
    public void Init(Action<int> callback)
    {
        WX.InitSDK(callback);
#if UNITY_EDITOR
        return;
#endif
        this.bannerAd = WX.CreateFixedBottomMiddleBannerAd(this.bannerAdUnitId, 30, 30);
        Debug.Log("banner ad is inited:" + bannerAd.ToString());
        this.interstitialAd = WX.CreateInterstitialAd(new WXCreateInterstitialAdParam()
        {
            adUnitId = interstitalAdUnitId
        });
        LoadIntestitial();

        this.rewardedVideoAd = WX.CreateRewardedVideoAd(new WXCreateRewardedVideoAdParam()
        {
            adUnitId = rewardedAdUnitId
        });
        this.rewardedVideoAd.Load();

        WX.SetPreferredFramesPerSecond(60);

        InitWXForRank();
    }

    public void ShowBanner()
    {
        if (this.bannerAd != null)
            this.bannerAd.Show();
    }

    public void HideBanner()
    {
        if (this.bannerAd != null)
            this.bannerAd.Hide();
    }

    public Boolean IsBannerShowing()
    {
        return false;
    }

    public bool IsInterstitialReady()
    {
        return this.interstitialAd != null && this.intestitalReady;
    }

    public void ShowInterstitial(Action<bool>callBack)
    {
        if (this.interstitialAd != null && this.intestitalReady)
        {
            
            this.interstitialAd.onCloseAction = () =>
            {
                callBack(true);
                LoadIntestitial();
            };
            this.interstitialAd.Show(success =>
            {
                
            }, failed =>
            {
                callBack(false);
            });
        }
        else
        {
            LoadIntestitial();
            callBack(false);
        }
            
    }

    void LoadIntestitial()
    {
        this.intestitalReady = false;
        this.interstitialAd.Load((success) =>
        {
            this.intestitalReady = true;
        }, failed =>
        {
            this.intestitalReady = false;
        });
    }

    public void ShowRewarded(Action<Boolean> callback)
    {
        if (this.rewardedVideoAd == null)
        {
            Debug.Log("rewarded ad is not init");
            return;
        }
        this.rewardedVideoAd.Show((WXTextResponse success) =>
        {

        }, (WXTextResponse failed) =>
        {
            callback(false);
        }
        );

        this.rewardedVideoAd.onCloseAction = null;

        this.rewardedVideoAd.OnClose(res =>
        {
            // 用户点击了【关闭广告】按钮
            // 小于 2.1.0 的基础库版本，res 是一个 undefined
            if (res != null && res.isEnded || res == null)
            {
                // 正常播放结束，可以下发游戏奖励
                callback(true);
            }
            else
            {
                // 播放中途退出，不下发游戏奖励
                callback(false);
            }
        });
    }

    public void Login(Action<Boolean, String> callback)
    {
        this.loginCallback = callback;
        WX.Login(new LoginOption()
        {
            success = OnLoginSuccess,
            fail = OnLoginFailed,
            complete = OnLoginComplete
        });
    }

    void OnLoginSuccess(LoginSuccessCallbackResult result)
    {
        Debug.Log($"WX SDK OnLoginSuccess code:{result.code}");
        this.loginCallback.Invoke(true, result.code);
    }
    void OnLoginFailed(RequestFailCallbackErr err)
    {
        Debug.Log($"WX SDK OnLoginFailed errMsg:{err.errMsg}");
        this.loginCallback.Invoke(false, err.errMsg);
    }
    void OnLoginComplete(GeneralCallbackResult result)
    {
        Debug.Log("WX SDK OnLoginComplete");
    }


    public void Share(Action<bool> callback)
    {
        this.shareCallback = callback;
        this.shareTime = DateTime.Now;
        WXCanvas.ToTempFilePath(new WXToTempFilePathParam()
        {
            x = 0,
            y = 0,
            width = 375,
            height = 300,
            destWidth = 375,
            destHeight = 300,
            success = (result) =>
            {
                Debug.Log("ToTempFilePath success" + JsonUtility.ToJson(result));
                WX.ShareAppMessage(new ShareAppMessageOption()
                {
                    title = "方块要塞",
                    imageUrl = result.tempFilePath,
                   // query = $"user={App.Instance.LocalUser.User.user_id}"
                });
            },
            fail = (result) =>
            {
                Debug.Log("ToTempFilePath fail" + JsonUtility.ToJson(result));
            },
            complete = (result) =>
            {
                Debug.Log("ToTempFilePath complete" + JsonUtility.ToJson(result));
            },
        });

        //InviteImgData inviteImgData = InviteImgConfig.Instance.GetRandomData();
        //WX.ShareAppMessage(new ShareAppMessageOption()
        //{
        //    title = inviteImgData.title,
        //    imageUrlId = inviteImgData.urlID,
        //    imageUrl = inviteImgData.url,
        //    query = $"user={App.Instance.LocalUser.User.user_id}"
        //});
    }

    /// <summary>
    /// 获取有效启动时的数据，如果玩家是通过分享链接进入的，通过query可以得到
    /// </summary>
    /// <param name="callBack"></param>
    public Dictionary<string, string> GetLaunchOptionsSync()
    {
        Dictionary<string, string> datas = new Dictionary<string, string>();
#if UNITY_EDITOR
        return datas;
#endif
        LaunchOptionsGame launchOptionsGame = WX.GetLaunchOptionsSync();
        if (launchOptionsGame != null)
        {
            datas = launchOptionsGame.query;
        }
        return datas;
    }

    public void VibrateShort()
    {

        if (vibrating) return;
        vibrating = true;
        WX.VibrateShort(new VibrateShortOption()
        {
            complete = (GeneralCallbackResult res) =>
            {
                vibrating = false;
            },
        });
    }
    public void VibrateLong()
    {
        if (vibrating) return;
        vibrating = true;
        WX.VibrateLong(new VibrateLongOption()
        {
            complete = (GeneralCallbackResult res) =>
            {
                vibrating = false;
            }
        });
    }

    public void RequirePrivacyAuthorize(Action<bool> callBack)
    {
        WX.RequirePrivacyAuthorize(new RequirePrivacyAuthorizeOption()
        {
            success = (result) =>
            {
                Debug.Log("RequirePrivacyAuthorize success");
                callBack?.Invoke(true);
            },
            fail = (result) =>
            {
                Debug.Log($"RequirePrivacyAuthorize fail errMsg:{result.errMsg}");
                callBack?.Invoke(false);
            }
        });
    }

    public void HandleNeedPrivacyAuthorization()
    {
        //WX.onNeedPrivacyAuthorization(resolve =>
        //{
        //    // ------ 自定义设置逻辑 ------ 
        //    // TODO：开发者弹出自定义的隐私弹窗（如果是勾选样式，开发者应在此实现自动唤出隐私勾选页面）
        //    // 页面展示给用户时，开发者调用 resolve({ event: 'exposureAuthorization' }) 告知平台隐私弹窗页面已曝光
        //    // 用户表示同意后，开发者调用 resolve({ event: 'agree' }) 告知平台用户已经同意，resolve要求用户有过点击行为。
        //    // 用户表示拒绝后，开发者调用 resolve({ event: 'disagree' }) 告知平台用户已经拒绝，resolve要求用户有过点击行为。
        //    // 是否需要控制间隔以及间隔时间，开发者可以自行实现
        //    // 勾选样式应以用户确认按钮的点击为准，无需每次勾选都上报
        //    // 如果需要主动弹窗见wx.requirePrivacyAuthorize
        //})

    }

    public void GetSettingUserInfo(Action<bool> callBack)
    {
        WX.GetSetting(new GetSettingOption()
        {
            success = (result) =>
            {
                Debug.Log($"GetSettingUserInfo success,authSetting:{JsonConvert.SerializeObject(result.authSetting)}");
                string userInfoKey = "scope.userInfo";
                if (result.authSetting.ContainsKey(userInfoKey) && result.authSetting[userInfoKey])
                {
                    callBack?.Invoke(true);
                }
                else
                {
                    //callBack?.Invoke(false);
                    //AuthorizeUserInfo(callBack);
                    CreateUserInfoBtn(callBack);
                }
            },
            fail = (result) =>
            {
                Debug.Log($"GetSettingUserInfo fail errMsg:{result.errMsg}");
                callBack?.Invoke(false);
            },
        });
    }

    public void AuthorizeUserInfo(Action<bool> callBack)
    {
        WX.Authorize(new AuthorizeOption()
        {
            scope = "scope.userInfo",
            success = (result) =>
            {
                Debug.Log("AuthorizeUserInfo success");
                callBack?.Invoke(true);
            },
            fail = (result) =>
            {
                Debug.Log($"AuthorizeUserInfo fail errMsg:{result.errMsg}");
                callBack?.Invoke(false);
            },
        });
    }

    public void CreateUserInfoBtn(Action<bool> callBack)
    {
        Debug.Log("CreateUserInfoBtn...... ");
        if (createButtonAction != null)
        {
            createButtonAction.Invoke(true);
        }
        WXUserInfoButton infoButton = CreateUserInfoButton();
        infoButton.OnTap(response =>
        {
            Debug.Log($"CreateUserInfoBtn OnTap errCode:{response.errCode} {response.errMsg},nickName:{response.userInfo.nickName}");
            if (!string.IsNullOrEmpty(response.userInfo.nickName))
            {
                infoButton.Hide();
                callBack?.Invoke(true);
            }
            else
            {
                infoButton.Hide();
                callBack?.Invoke(false);
            }

        });
        infoButton.Show();
    }

    public static WXUserInfoButton CreateUserInfoButton()
    {
        return WX.CreateUserInfoButton(10, 10, 1800, 2800, "zh_CN", true);
    }

    public void GetUserInfo(Action<bool, UserInfo> callBack)
    {
        WX.GetUserInfo(new GetUserInfoOption()
        {
            success = (result) =>
            {
                Debug.Log("GetUserInfo success");
                callBack?.Invoke(true, result.userInfo);
            },
            fail = (result) =>
            {
                Debug.Log($"GetUserInfo fail errMsg:{result.errMsg}");
                callBack?.Invoke(false, null);
            },
        });
    }

    public void SetClipboardData(string dataStr)
    {
        WX.SetClipboardData(new SetClipboardDataOption()
        {
            data = dataStr,
            success = (result) =>
            {
                Debug.Log("SetClipboardData success");
            },
            fail = (result) =>
            {
                Debug.Log($"SetClipboardData fail errMsg:{result.errMsg}");
            },
        });
    }

    /// <summary>
    /// 用于分支相关的UI组件（一般是按钮）相关事件的上报，事件目前有曝光、点击两种
    /// </summary>
    /// <param name="branchId">分支ID，在「小程序管理后台」获取</param>
    /// <param name="branchDim">自定义维度</param>
    /// <param name="eventType">事件类型，1：曝光； 2：点击</param>
    public void ReportUserBehaviorBranchAnalytics(string branchId, string branchDim, int eventType)
    {
#if UNITY_EDITOR
        return;
#endif
        WX.ReportUserBehaviorBranchAnalytics(branchId, branchDim, eventType);
    }

    public void ReportScene(WXReportSceneType sceneType)
    {
#if UNITY_EDITOR
        return;
#endif
        if (reportSceneFinish)
        {
            return;
        }
        WX.ReportScene(new ReportSceneOption()
        {
            sceneId = (int)sceneType,
        });

        if (sceneType == WXReportSceneType.CheckPlayerOP)
        {
            reportSceneFinish = true;
        }
    }

    #region 客服
    /// <summary>
    /// 打开微信客服 https://developers.weixin.qq.com/minigame/dev/api/open-api/service-chat/wx.openCustomerServiceChat.html
    /// 必传参数	
    /// extInfo 客服链接 	 
    /// corpId 企业ID
    /// </summary>
    public void OpenCustomerServiceChat()
    {
        //
        WX.OpenCustomerServiceChat(new OpenCustomerServiceChatOption()
        {
            extInfo = new ExtInfoOption()
            {
                url = "https://work.weixin.qq.com/kfid/kfc1a427dc7c3098d68",
            },
            corpId = "ww570bf9a3a1c18033",
            fail = (result) => { },
            success = (result) => { },
        });
    }
    #endregion

    #region OpenContext & Ranking

    public void InitRankCanvas(RawImage RankBody, Canvas RankingCanvas, GameObject rankMask)
    {
        this.RankingCanvas = RankingCanvas;
        this.RankBody = RankBody;
        this.RankMask = rankMask;
    }
    void InitWXForRank()
    {
        /**
         * 使用群排行功能需要特殊设置分享功能，详情可见链接
         * https://developers.weixin.qq.com/minigame/dev/guide/open-ability/share/share.html
         */
        WX.UpdateShareMenu(new UpdateShareMenuOption()
        {
            withShareTicket = true,
            isPrivateMessage = true,
        });
        
        /**
         * 群排行榜功能需要配合 WX.OnShow 来使用，整体流程为：
         * 1. WX.UpdateShareMenu 分享功能；
         * 2. 监听 WX.OnShow 回调，如果存在 shareTicket 且 query 里面带有启动特定 query 参数则为需要展示群排行的场景
         * 3. 调用 WX.ShowOpenData 和 WX.GetOpenDataContext().PostMessage 告知开放数据域侧需要展示群排行信息
         * 4. 开放数据域调用 wx.getGroupCloudStorage 接口拉取获取群同玩成员的游戏数据
         * 5. 将群同玩成员数据绘制到 sharedCanvas
         */
        WX.OnShow((res) =>
        {
            string shareTicket = res.shareTicket;
            Dictionary<string, string> query = res.query;

            if (!string.IsNullOrEmpty(shareTicket) && query != null && query["minigame_action"] == "show_group_list")
            {
                InitOpenDataContext();
                ShowOpenData();

                OpenDataMessage msgData = new OpenDataMessage();
                msgData.type = "showGroupFriendsRank";
                msgData.shareTicket = shareTicket;

                string msg = JsonUtility.ToJson(msgData);

                openDataContext.PostMessage(msg);
            }

            CheckShareSuccess();
        });
    }

    void CheckShareSuccess()
    {
        if (shareCallback != null)
        {
            TimeSpan span = DateTime.Now - shareTime;
            if (span.TotalSeconds > 5)
            {
                shareCallback?.Invoke(true);
            }
            shareCallback?.Invoke(false);
            shareCallback = null;
        }
    }
    void InitOpenDataContext()
    {
        if (openDataContext == null)
        {
            openDataContext = WX.GetOpenDataContext(new OpenDataContextOption
            {
                sharedCanvasMode = CanvasType.ScreenCanvas
            });
        }
    }

    public void ShowFriendRand()
    {
        Debug.Log("ShowFriendRand");
        RankMask.gameObject.SetActive(true);
        InitOpenDataContext();
        ShowOpenData();

        OpenDataMessage msgData = new OpenDataMessage();
        msgData.type = "showFriendsRank";
        Debug.Log("ShowFriendRand post message");
        string msg = JsonUtility.ToJson(msgData);
        openDataContext.PostMessage(msg);
    }
    
    public void HideFriendRand()
    {
        RankMask.gameObject.SetActive(false);
        RankBody.gameObject.SetActive(false);
        WX.HideOpenData();
    }

    public void ShareFriendRand()
    {
        WXSharedCanvas.ToTempFilePath(new WXToTempFilePathParam()
        {
            x = 0,
            y = 0,
            width = 960,
            height = 800,
            destWidth = 375,
            destHeight = 300,
            success = (result) =>
            {
                Debug.Log("ToTempFilePath success" + JsonUtility.ToJson(result));
                WX.ShareAppMessage(new ShareAppMessageOption()
                {
                    title = "不服就来追我",
                    imageUrl = result.tempFilePath,
                  //  query = $"user={App.Instance.LocalUser.User.user_id}"
                });
            },
            fail = (result) =>
            {
                Debug.Log("ToTempFilePath fail" + JsonUtility.ToJson(result));
            },
            complete = (result) =>
            {
                Debug.Log("ToTempFilePath complete" + JsonUtility.ToJson(result));
            },
        });
    }
    /**
     * 上报分数
     */
    public void ReportScore(int score)
    {
        OpenDataMessage msgData = new OpenDataMessage();
        msgData.type = "setUserRecord";
        msgData.score = score;

        string msg = JsonUtility.ToJson(msgData);

        Debug.Log(msg);
        InitOpenDataContext();
        openDataContext.PostMessage(msg);
    }

    void ShowOpenData()
    {
        
        // 
        // 注意这里传x,y,width,height是为了点击区域能正确点击，x,y 是距离屏幕左上角的距离，宽度传 (int)RankBody.rectTransform.rect.width是在canvas的UI Scale Mode为 Constant Pixel Size的情况下设置的。
        /**
         * 如果父元素占满整个窗口的话，pivot 设置为（0，0），rotation设置为180，则左上角就是离屏幕的距离
         * 注意这里传x,y,width,height是为了点击区域能正确点击，因为开放数据域并不是使用 Unity 进行渲染而是可以选择任意第三方渲染引擎
         * 所以开放数据域名要正确处理好事件处理，就需要明确告诉开放数据域，排行榜所在的纹理绘制在屏幕中的物理坐标系
         * 比如 iPhone Xs Max 的物理尺寸是 414 * 896，如果排行榜被绘制在屏幕中央且物理尺寸为 200 * 200，那么这里的 x,y,width,height应当是 107,348,200,200
         * x,y 是距离屏幕左上角的距离，宽度传 (int)RankBody.rectTransform.rect.width是在canvas的UI Scale Mode为 Constant Pixel Size的情况下设置的
         * 如果是Scale With Screen Size，且设置为以宽度作为缩放，则要这要做一下换算，比如canavs宽度为960，rawImage设置为200 则需要根据 referenceResolution 做一些换算
         * 不过不管是什么屏幕适配模式，这里的目的就是为了算出 RawImage 在屏幕中绝对的位置和尺寸
         */
        
        CanvasScaler scaler = RankingCanvas.GetComponent<CanvasScaler>();
        var referenceResolution = scaler.referenceResolution;
        var p = RankBody.transform.position;
        WX.ShowOpenData(RankBody.texture, (int)p.x, Screen.height - (int)p.y, (int)((Screen.width / referenceResolution.x) * RankBody.rectTransform.rect.width), (int)((Screen.width / referenceResolution.x) * RankBody.rectTransform.rect.height));
 
    }
    #endregion
}

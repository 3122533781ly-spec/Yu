using Lei31;
using System;
using System.Linq;
using ProjectSpace.Lei31Utils.Scripts.Framework.App;
using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;
using UnityEngine;

namespace Redeem
{
    /// <summary>
    /// 考虑如何优化修改 最好是通过配置文件初始化数据 只存储看广告 邀请好友等条件数据，通过表格配置的ID来查询对应的数据
    /// </summary>
    public class RedeemSystem : GameSystem
    {
        public RedeemModel Model { get; private set; }

        private PersistenceData<string> modelStr;
        public PersistenceData<int> countryDataIndex; //国家配置下标 用来做ABtest(0基础配置，1配置B)

        public Action<PayPalEntryData> onNewPayPalEntryUnlock;
        public Action<GiftCardEntryData> onNewGiftCardEntryUnlock;

        private bool hasInit = false;

        public override void Destroy()
        {
        }

        public override void Init()
        {
            modelStr = new PersistenceData<string>(MakeKey("modelStr"), "");
            countryDataIndex = new PersistenceData<int>(MakeKey("countryDataIndex"), 0);
            ReadModel();
        }

        private string MakeKey(string key)
        {
            return $"RedeemSystem_{key}";
        }

        private void ReadModel()
        {
            if (!string.IsNullOrEmpty(modelStr.Value))
            {
                Model = JsonUtility.FromJson<RedeemModel>(modelStr.Value);
            }
            else
            {
                Model = new RedeemModel();
                SaveModel();
            }
        }

        private void SaveModel()
        {
            if (Model != null)
            {
                string jsonStr = JsonUtility.ToJson(Model);
                modelStr.Value = jsonStr;
            }
        }

        /// <summary>
        /// 获取正在筹集美元的档位，需要的美元数量
        /// </summary>
        /// <returns></returns>
        public int GetPayPalWaitToRedeemMoneyValue()
        {
            foreach (var item in Model.payPalEntryDatas)
            {
                if (item.state == RedeemEntryState.Underway)
                {
                    return item.GetRedeemData().money;
                }
            }

            return 0;
        }

        public void SetCountryCode(string country)
        {
            //2023-10-21 ZQ 如果之前存储过country，就不变了
            //if (!string.IsNullOrEmpty(country) && Model.country != country)
            if (!string.IsNullOrEmpty(country) && string.IsNullOrEmpty(Model.country))
            {
                Model.country = country;
                SaveModel();
            }

            StaticModule.RedeemCountry(Model.country);
        }

        public RedeemCountryData GetCountryData()
        {
            Debug.Log($"GetCountryData countryDataIndex:{countryDataIndex.Value},country:{Model.country}");
            if (countryDataIndex.Value == 0)
            {
                return RedeemCountryConfig.Instance.All.Find(ret => { return ret.countryCode == Model.country; });
            }
            else if (countryDataIndex.Value == 1)
            {
                return RedeemCountryBConfig.Instance.All.Find(ret => { return ret.countryCode == Model.country; });
            }
            else
            {
                return RedeemCountryCConfig.Instance.All.Find(ret => { return ret.countryCode == Model.country; });
            }
        }

        /// <summary>
        /// 提现系统是否可以开启，由服务器，玩家所有在国家，客户端配置（需要有所在国家的配置数据）共同决定
        /// </summary>
        /// <returns></returns>
        public bool CheckRedeemValid()
        {
            return GetCountryData() != null;
        }

        public void ChangeMoney(float value)
        {
            if (value != 0f)
            {
                Game.Instance.CurrencyModel.RewardMoney(value);
            }
        }

        public void ChangeDiamond(int value)
        {
            if (value != 0)
            {
                Game.Instance.CurrencyModel.Diamond.Value += value;
            }
        }

        public void ChangePayPalAccount(string account)
        {
            if (!string.IsNullOrEmpty(account) && Model.paypalAccount != account)
            {
                Model.paypalAccount = account;
                SaveModel();
            }
        }

        public void InitRedeem()
        {
            if (CheckRedeemValid() && !hasInit)
            {
                RedeemCountryData countryData = GetCountryData();

                if (Model.payPalEntryDatas.Count <= 0)
                {
                    Model.AddPayPalEntryData(countryData.moneyConfigs[0], RedeemEntryState.Underway);
                    SaveModel();
                }

                if (Model.giftCardEntryDatas.Count <= 0)
                {
                    Model.AddGiftCardEntryData(countryData.giftCardConfigs[0], RedeemEntryState.Underway);
                    SaveModel();
                }

                CheckRedeem();
                hasInit = true;
                Debug.Log($"InitRedeem......");
            }
        }


        public void CheckRedeem()
        {
            RedeemCountryData countryData = GetCountryData();
            for (int i = 0; i < Model.payPalEntryDatas.Count; i++)
            {
                Debug.Log($"CheckRedeem CheckPayPal i :{i}");
                Model.payPalEntryDatas[i].CheckPayPal(Game.Instance.CurrencyModel.GetCurrentMoney(),
                    countryData.GetMoneyConfig(i));
                if (Model.payPalEntryDatas[i].state == RedeemEntryState.NeedFinishCondition)
                {
                    int serverID = ((PayPalEntryData) (Model.payPalEntryDatas[i])).serverID;
                    Debug.Log(
                        $"CheckRedeem CheckPayPal i :{i},inviteLink:{Model.payPalEntryDatas[i].inviteLink},serverID:{serverID},redeemDataID:{Model.payPalEntryDatas[i].redeemDataID}");
                    if (serverID <= 0)
                    {
                        //修复serverID 为0的bug
                        HandleApplyRedeem(Model.payPalEntryDatas[i].GetRedeemData().money, Model.paypalAccount,
                            (success, applyRedeemResponseData) =>
                            {
                                if (success && applyRedeemResponseData != null)
                                {
                                    ((PayPalEntryData) (Model.payPalEntryDatas[i])).serverID =
                                        applyRedeemResponseData.id;
                                    Model.payPalEntryDatas[i].inviteLink = "";
                                    GenerateRedeemInviteLink(Model.payPalEntryDatas[i]);
                                }
                            });
                    }
                    else
                    {
                        GenerateRedeemInviteLink(Model.payPalEntryDatas[i]);
                    }
                }
            }

            for (int i = 0; i < Model.giftCardEntryDatas.Count; i++)
            {
                Debug.Log($"CheckRedeem CheckGiftCard i :{i}");
                Model.giftCardEntryDatas[i].CheckGiftCard(Game.Instance.CurrencyModel.Diamond.Value,
                    countryData.GetGiftCardConfig(i));
                if (Model.giftCardEntryDatas[i].state == RedeemEntryState.NeedFinishCondition)
                {
                    Debug.Log($"CheckRedeem  CheckGiftCard i :{i},inviteLink:{Model.giftCardEntryDatas[i].inviteLink}");
                    GenerateRedeemInviteLink(Model.giftCardEntryDatas[i]);
                }
            }
        }

        public void JumpNowCondition()
        {
            foreach (var item in Model.payPalEntryDatas)
            {
                if (item.state == RedeemEntryState.NeedFinishCondition)
                {
                    item.EnterNextCondition();
                }
            }

            foreach (var item in Model.giftCardEntryDatas)
            {
                if (item.state == RedeemEntryState.NeedFinishCondition)
                {
                    item.EnterNextCondition();
                }
            }
        }

        /// <summary>
        /// 申请提现
        /// </summary>
        /// <param name="entryData"></param>
        public void HandleApplyRedeem(RedeemEntryBaseData entryData)
        {
            if (entryData.state == RedeemEntryState.CanApplyRedeem)
            {
                if (entryData.type == RedeemEntryType.PayPal)
                {
                    if (string.IsNullOrEmpty(Model.paypalAccount))
                    {
                        DialogManager.Instance.GetDialog<RedeemAccountDialog>().Show(() =>
                        {
                            HandleConfirmAccount(entryData);
                        });
                    }
                    else
                    {
                        HandleConfirmAccount(entryData);
                    }
                }
                else if (entryData.type == RedeemEntryType.GiftCard)
                {
                    ChangeDiamond(-((GiftCardRedeemData) (entryData.GetRedeemData())).diamond);
                    entryData.ChangeState(RedeemEntryState.NeedFinishCondition);

                    //如果后续还有档位，则解锁 如果没有就一直循环最后一个
                    RedeemCountryData countryData = GetCountryData();
                    if (countryData.giftCardConfigs.Count > Model.giftCardEntryDatas.Count)
                    {
                        Model.AddGiftCardEntryData(countryData.giftCardConfigs[Model.giftCardEntryDatas.Count],
                            RedeemEntryState.Underway);
                    }
                    else
                    {
                        Model.AddGiftCardEntryData(countryData.giftCardConfigs.Last(), RedeemEntryState.Underway);
                    }

                    SaveModel();

                    onNewGiftCardEntryUnlock?.Invoke(Model.giftCardEntryDatas.Last());
                    GenerateRedeemInviteLink(entryData);
                }
            }
        }

        private void HandleConfirmAccount(RedeemEntryBaseData entryData)
        {
            DialogManager.Instance.GetDialog<RedeemConfirmAccountDialog>().Show(entryData.GetRedeemData().money,
                () =>
                {
                    DialogManager.Instance.GetDialog<RedeemAccountDialog>().Show(() =>
                    {
                        HandleConfirmAccount(entryData);
                    });
                },
                () =>
                {
                    DialogManager.Instance.GetDialog<WaitingDialog>().Activate();
                    HandleApplyRedeem(entryData.GetRedeemData().money, Model.paypalAccount,
                        (success, applyRedeemResponseData) =>
                        {
                            DialogManager.Instance.GetDialog<WaitingDialog>().Deactivate();
                            if (success && applyRedeemResponseData != null)
                            {
                                ChangeMoney(-entryData.GetRedeemData().money);
                                entryData.ChangeState(RedeemEntryState.NeedFinishCondition);
                                ((PayPalEntryData) entryData).serverID = applyRedeemResponseData.id;

                                Debug.Log(
                                    $"HandleApplyRedeem serverID:{((PayPalEntryData) entryData).serverID},redeemDataID:{entryData.redeemDataID}");

                                //如果后续还有档位，则解锁 如果没有就一直循环最后一个
                                RedeemCountryData countryData = GetCountryData();
                                if (countryData.moneyConfigs.Count > Model.payPalEntryDatas.Count)
                                {
                                    Model.AddPayPalEntryData(countryData.moneyConfigs[Model.payPalEntryDatas.Count],
                                        RedeemEntryState.Underway);
                                }
                                else
                                {
                                    Model.AddPayPalEntryData(countryData.moneyConfigs.Last(),
                                        RedeemEntryState.Underway);
                                }

                                SaveModel();
                                onNewPayPalEntryUnlock?.Invoke(Model.payPalEntryDatas.Last());

                                GenerateRedeemInviteLink(entryData);
                            }
                        });
                });
        }

        private void GenerateRedeemInviteLink(RedeemEntryBaseData entryData)
        {
            if (string.IsNullOrEmpty(entryData.inviteLink))
            {
                if (entryData.type == RedeemEntryType.PayPal)
                {
                    SDKManager.AppsFlyerGenerateRedeemInviteLink(Game.Instance.LocalUser.User.user_id.ToString(),
                        ((PayPalEntryData) entryData).serverID.ToString(), link =>
                        {
                            entryData.inviteLink = link;
                            SaveModel();
                            Debug.Log(
                                $"RedeemSystem GenerateRedeemInviteLink.  link:{link},serverID:{((PayPalEntryData) entryData).serverID}");
                        });
                }
                else
                {
                    if (!string.IsNullOrEmpty(Game.Instance.LocalUser.AppsFlyShareLink))
                    {
                        entryData.inviteLink = Game.Instance.LocalUser.AppsFlyShareLink;
                        SaveModel();
                    }
                }
            }
        }

        public void HandleConfirmRedeem(RedeemEntryBaseData entryData)
        {
            if (entryData.state == RedeemEntryState.CanConfirm)
            {
                if (entryData.type == RedeemEntryType.PayPal)
                {
                    DialogManager.Instance.GetDialog<WaitingDialog>().Activate();
                    ConfirmRedeem(((PayPalEntryData) entryData).serverID, (success, response) =>
                    {
                        DialogManager.Instance.GetDialog<WaitingDialog>().Deactivate();
                        if (success)
                        {
                            entryData.ChangeState(RedeemEntryState.UnderReview);
                        }
                    });
                }
                else if (entryData.type == RedeemEntryType.GiftCard)
                {
                    entryData.ChangeState(RedeemEntryState.UnderReview);
                }
            }
        }

        /// <summary>
        /// Again 客户端再次向服务器发起提现申请 客户端重新开始任务（不需要再减美元）
        /// </summary>
        /// <param name="entryData"></param>
        public void HandleAgain(RedeemEntryBaseData entryData)
        {
            if (entryData.type == RedeemEntryType.PayPal)
            {
                DialogManager.Instance.GetDialog<WaitingDialog>().Activate();
                HandleApplyRedeem(entryData.GetRedeemData().money, Model.paypalAccount,
                    (success, applyRedeemResponseData) =>
                    {
                        DialogManager.Instance.GetDialog<WaitingDialog>().Deactivate();
                        if (success && applyRedeemResponseData != null)
                        {
                            entryData.ResetCondition();
                            ((PayPalEntryData) entryData).serverID = applyRedeemResponseData.id;
                            entryData.ChangeState(RedeemEntryState.NeedFinishCondition);
                            entryData.inviteLink = "";
                            GenerateRedeemInviteLink(entryData);

                            StaticModule.RedeemAgain(entryData.GetRedeemData().money);
                        }
                    });
            }
        }


        #region 任务 优先级 美元档位》钻石档位

        private void HandleAddConditionCount(RedeemEntryState state, RedeemConditionType type, int count)
        {
            PayPalEntryData payPalEntryData = Model.payPalEntryDatas.Find(ret =>
            {
                return ret.state == state &&
                       ret.nowConditionProgressData.type == type;
            });

            if (payPalEntryData != null)
            {
                payPalEntryData.nowConditionProgressData.AddCount(count);
                payPalEntryData.CheckNowConditionFinish();
            }

            SaveModel();
        }

        private void HandleAddConditionCount_Gift(RedeemEntryState state, RedeemConditionType type, int count)
        {
            GiftCardEntryData giftCardEntryData = Model.giftCardEntryDatas.Find(ret =>
            {
                return ret.state == state &&
                       ret.nowConditionProgressData.type == type;
            });

            if (giftCardEntryData != null)
            {
                giftCardEntryData.nowConditionProgressData.AddCount(count);
                giftCardEntryData.CheckNowConditionFinish();
            }

            SaveModel();
        }

        public void AddWatchADCount(int count = 1)
        {
            HandleAddConditionCount(RedeemEntryState.NeedFinishCondition, RedeemConditionType.WatchAD, count);
            HandleAddConditionCount_Gift(RedeemEntryState.NeedFinishCondition, RedeemConditionType.WatchAD, count);
        }

        public void AddInvitePlayerCount(int count = 1)
        {
            HandleAddConditionCount(RedeemEntryState.NeedFinishCondition, RedeemConditionType.InvitePlayer, count);
            HandleAddConditionCount_Gift(RedeemEntryState.NeedFinishCondition, RedeemConditionType.InvitePlayer, count);
        }

        public void AddPlayGameCount(int count = 1)
        {
            HandleAddConditionCount(RedeemEntryState.NeedFinishCondition, RedeemConditionType.PlayGame, count);
            HandleAddConditionCount_Gift(RedeemEntryState.NeedFinishCondition, RedeemConditionType.PlayGame, count);
        }

        #endregion

        #region 网络请求

        /// <summary>
        /// 上报广告价值，再广告价值累计超过0.01美元后再上报
        /// </summary>
        /// <param name="ad_revenue"></param>
        public void ReportADRevenue(float ad_revenue)
        {
            Model.adRevenueSum += ad_revenue;
            if (Model.adRevenueSum >= 0.01f)
            {
                JSONObject obj = new JSONObject();
                obj.Add("ad_revenue", Model.adRevenueSum);
                string data = obj.ToString();
                UnityNetworkManager.Instance.Post("user/adrevenue/report", data,
                    (err, response) => { Debug.Log($"ReportADRevenue data:{data},err:{err},response:{response}"); });
                Model.adRevenueSum = 0f;
            }
        }

        public void HandleRecordReferUser(int userID, int redeemID)
        {
            Model.referUserID = userID;
            Model.referUserRedeemID = redeemID;
            CheckCountryAndSetReferUser();
        }

        /// <summary>
        /// 服务器拿到国家信息后，才能设置邀请用户信息
        /// </summary>
        public void CheckCountryAndSetReferUser()
        {
            if (!string.IsNullOrEmpty(Model.country))
            {
                HandleSetReferUser();
            }
        }

        /// <summary>
        /// 提交提现任务的邀请信息 新用户只需成功提交一次
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="redeemID">为申请提现时，服务器端生成的id，通过分享链接把userID redeemID传递给新用户</param>
        public void HandleSetReferUser()
        {
            Debug.Log(
                $"RedeemSystem HandleSetReferUser userID:{Model.referUserID},redeemID:{Model.referUserRedeemID}，country：{Model.country}");
            //test
            //hasSetReferUser.Value = false;

            if (!Model.hasSetReferUser && Model.referUserID > 0 && Model.referUserRedeemID > 0)
            {
                SetReferUser(Model.referUserID, Model.referUserRedeemID, (success, response) =>
                {
                    if (success)
                    {
                        Model.hasSetReferUser = true;
                    }
                });
            }
        }

        /// <summary>
        /// 提交提现任务的邀请信息
        /// 在客户端安装启动后，通过AF解析到邀请信息，得到提现相关内容再上报
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="redeemID">为申请提现时，服务器端生成的id（ 申请提现或者查询提现历史接口可以得到这个ID）</param>
        private void SetReferUser(int userID, int redeemID, Action<bool, string> actionResult)
        {
            JSONObject obj = new JSONObject();
            obj.Add("refer_user", userID);
            obj.Add("redeem_id", redeemID);
            string data = obj.ToString();
            UnityNetworkManager.Instance.Post("user/refer/redeem/set", data, (err, response) =>
            {
                Debug.Log($"SetReferUser data:{data},err:{err},response:{response}");
                actionResult?.Invoke(!err, response);
            });
        }

        public void HandleApplyRedeem(int cashCount, string paypalAccount,
            Action<bool, ApplyRedeemResponseDetailData> actionResult)
        {
            ApplyRedeem(cashCount, paypalAccount, (success, response) =>
            {
                ApplyRedeemResponseDetailData applyRedeemResponseData = null;
                if (success)
                {
                    applyRedeemResponseData = JsonUtility.FromJson<ApplyRedeemResponseDetailData>(response);
                }

                actionResult?.Invoke(success, applyRedeemResponseData);

                if (success)
                {
                    StaticModule.RedeemApply(cashCount);
                }
            });
        }

        /// <summary>
        /// 申请提现 在玩家拥有美元金额满足条件，并填写了paypal账号后可以申请
        /// 申请后，进入任务模式，完成任务后，再确认提现
        /// </summary>
        /// <param name="cashCount"></param>
        /// <param name="paypalAccount"></param>
        private void ApplyRedeem(int cashCount, string paypalAccount, Action<bool, string> actionResult)
        {
            JSONObject obj = new JSONObject();
            obj.Add("cash", cashCount);
            obj.Add("paypal", paypalAccount);
            string data = obj.ToString();
            UnityNetworkManager.Instance.Post("user/redeem/do", data, (err, response) =>
            {
                Debug.Log($"ApplyRedeem data:{data},err:{err},response:{response}");
                actionResult?.Invoke(!err, response);
            });
        }

        public void HandleGetRedeemList(Action<bool, RedeemListResponseData> actionResult)
        {
            GetRedeemList((success, response) =>
            {
                RedeemListResponseData redeemListResponseData = null;
                if (success)
                {
                    redeemListResponseData = JsonUtility.FromJson<RedeemListResponseData>(response);
                    RefreshPayPalEntryByServerData(redeemListResponseData);
                }

                actionResult?.Invoke(success, redeemListResponseData);
            });
        }

        private void RefreshPayPalEntryByServerData(RedeemListResponseData responseData)
        {
            if (responseData != null && responseData.items != null)
            {
                bool needSave = false;
                foreach (var item in responseData.items)
                {
                    PayPalEntryData targetEntryData = Model.payPalEntryDatas.Find(ret =>
                    {
                        return ret.serverID == item.id;
                    });
                    if (targetEntryData != null)
                    {
                        if (targetEntryData.state == RedeemEntryState.NeedFinishCondition)
                        {
                            if (targetEntryData.nowConditionProgressData.type == RedeemConditionType.InvitePlayer ||
                                targetEntryData.nowConditionProgressData.type == RedeemConditionType.ActivePlayer)
                            {
                                targetEntryData.nowConditionProgressData.invitePlayer = item.invite_users;
                                targetEntryData.nowConditionProgressData.activePlayer = item.active_users;
                                needSave = true;
                                targetEntryData.CheckNowConditionFinish();
                                targetEntryData.onDataChanged?.Invoke();
                            }
                        }
                        else if (targetEntryData.state == RedeemEntryState.UnderReview)
                        {
                            if (item.status == (int) RedeemServerStatus.Rejected)
                            {
                                targetEntryData.ChangeState(RedeemEntryState.Rejected);
                                targetEntryData.serverNote = item.note;
                                needSave = true;
                            }
                            else if (item.status == (int) RedeemServerStatus.Approved)
                            {
                                targetEntryData.ChangeState(RedeemEntryState.Approved);
                                needSave = true;
                            }
                            else if (item.status == (int) RedeemServerStatus.Paid)
                            {
                                targetEntryData.ChangeState(RedeemEntryState.FinishRedeem);
                                needSave = true;
                            }
                        }
                        else if (targetEntryData.state == RedeemEntryState.Rejected)
                        {
                            if (targetEntryData.serverNote != item.note)
                            {
                                targetEntryData.serverNote = item.note;
                                needSave = true;
                            }
                        }
                        else if (targetEntryData.state == RedeemEntryState.Approved)
                        {
                            if (item.status == (int) RedeemServerStatus.Paid)
                            {
                                targetEntryData.ChangeState(RedeemEntryState.FinishRedeem);
                                needSave = true;
                            }
                        }
                    }
                }

                if (needSave)
                {
                    SaveModel();
                }
            }
        }

        /// <summary>
        /// 查询提现历史  （查询邀请进度（active_users），提现状态用这里）
        /// Response: {
        ///{
        ///  "items":[{
        ///    "id":2,
        ///    "redeem_date":"2023-10-10 16:43:34",
        ///      "amount":10,
        ///      "paypal":"13012959@qq.com",
        ///      "invite_users":10,  //邀请的用户数量，根据前端显示最大值就可以了
        ///      "active_users": 5,//激活的用户数量，邀请用户中广告价值达到1美金（暂定，后台可以修改），前端显示值不超过invite_users。
        ///     "status":0,  
        ///     /**
        /// * 0: 已提交
        ///  * 1: 已确认
        /// * 2: 已审核
        /// * 3: 已付款
        /// * 4: 已拒绝
        ///  */
        ///      "note": "" //如果拒绝 拒绝的详情提示在这里
        ///      }]}
        /// </summary>
        /// <param name="actionResult"></param>
        private void GetRedeemList(Action<bool, string> actionResult)
        {
            UnityNetworkManager.Instance.Get($"user/redeem/list", (err, response) =>
            {
                Debug.Log($"GetRedeemList  err:{err},response:{response}");
                actionResult?.Invoke(!err, response);
            });
        }

        /// <summary>
        /// 确认提现 
        /// </summary>
        /// <param name="redeemID">为申请提现时，服务器端生成的id（ 申请提现或者查询提现历史接口可以得到这个ID）</param>
        public void ConfirmRedeem(int redeemID, Action<bool, string> actionResult)
        {
            JSONObject obj = new JSONObject();
            obj.Add("redeem_id", redeemID);
            string data = obj.ToString();
            UnityNetworkManager.Instance.Post("user/redeem/confirm", data, (err, response) =>
            {
                Debug.Log($"ConfirmRedeem data:{data},err:{err},response:{response}");
                actionResult?.Invoke(!err, response);
            });
        }

        #endregion
    }
}
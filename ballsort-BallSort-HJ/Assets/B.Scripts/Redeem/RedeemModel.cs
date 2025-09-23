using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Redeem
{
    [Serializable]
    public class RedeemModel
    {
        public string country;

        public float adRevenueSum;
        public bool hasSetReferUser;
        public int referUserID;
        public int referUserRedeemID;
        public string paypalAccount;

        public List<PayPalEntryData> payPalEntryDatas = new List<PayPalEntryData>();
        public List<GiftCardEntryData> giftCardEntryDatas = new List<GiftCardEntryData>();

        public void AddPayPalEntryData(int redeemDataID, RedeemEntryState state)
        {
            payPalEntryDatas.Add(new PayPalEntryData(redeemDataID, state));
        }
        public void AddGiftCardEntryData(int redeemDataID, RedeemEntryState state)
        {
            giftCardEntryDatas.Add(new GiftCardEntryData(redeemDataID, state));
        }

    }

    public enum RedeemEntryState
    {
        Lock,//锁定
        Underway,//进行中
        CanApplyRedeem,//可以申请提现（金额够了，显示提现按钮）
        NeedFinishCondition,//发起提现后，显示后续需要完成的条件
        CanConfirm,//完成了所有条件，可以确认提现,需要玩家点击Confirm按钮
        UnderReview,//等待审核
        Rejected,//被拒绝 显示拒绝原因和Again按钮（再重新开始 所有条件重新再来一次 进入NeedFinishCondition状态）
        Approved,//已经批准
        FinishRedeem,//完成提现
    }

}
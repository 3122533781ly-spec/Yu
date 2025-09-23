using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Redeem
{
    [Serializable]
    public class PayPalEntryData : RedeemEntryBaseData
    {
        public int serverID;
        public string serverNote;

        public PayPalEntryData(int redeemDataID, RedeemEntryState state) : base(redeemDataID, state)
        {
            type = RedeemEntryType.PayPal;
            //ChangeState(state);
            //RefreshRedeemDataID(redeemDataID);
            //nowConditionIndex = 0;

            //MoneyRedeemData redeemData = GetRedeemData();
            //RedeemConditionData conditionData = redeemData.conditionDatas[nowConditionIndex];
            //nowConditionProgressData = new RedeemConditionProgressData(conditionData);
        }


        public override RedeemConfigBaseData GetRedeemData()
        {
            Debug.Log($"GetMoneyRedeemData this redeemDataID:{this.redeemDataID}");
            return MoneyRedeemConfig.Instance.GetConfigByID(redeemDataID);
        }

        public void CheckPayPal(float money, int redeemDataID)
        {
            RefreshRedeemDataID(redeemDataID);
            if (state == RedeemEntryState.Underway)
            {
                MoneyRedeemData moneyRedeemData = (MoneyRedeemData)GetRedeemData();
                if (money >= moneyRedeemData.money)
                {
                    ChangeState(RedeemEntryState.CanApplyRedeem);
                }
            }
            else if (state == RedeemEntryState.NeedFinishCondition)
            {
                CheckNowConditionFinish();
            }
        }

    }
}

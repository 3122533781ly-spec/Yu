using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Redeem
{
    [Serializable]
    public class GiftCardEntryData : RedeemEntryBaseData
    {
        public GiftCardEntryData(int redeemDataID, RedeemEntryState state) : base(redeemDataID, state)
        {
            type = RedeemEntryType.GiftCard;
            //ChangeState(state);
            //RefreshRedeemDataID(redeemDataID);
            //nowConditionIndex = 0;
            Debug.Log("GiftCardEntryData.....");
            //GiftCardRedeemData redeemData = GetGiftCardRedeemData();
            //RedeemConditionData conditionData = redeemData.conditionDatas[nowConditionIndex];
            //nowConditionProgressData = new RedeemConditionProgressData(conditionData);
        }

        public override RedeemConfigBaseData GetRedeemData()
        {
            Debug.Log($"GiftCardRedeemData this redeemDataID:{this.redeemDataID}");
            return GiftCardRedeemConfig.Instance.GetConfigByID(redeemDataID);
        }

        public GiftCardInfoData GetGiftCardInfoData()
        {
            return GiftCardInfoConfig.Instance.GetConfigByID(((GiftCardRedeemData) GetRedeemData()).cardID);
        }

        public void CheckGiftCard(int diamond, int redeemDataID)
        {
            RefreshRedeemDataID(redeemDataID);
            if (state == RedeemEntryState.Underway)
            {
                GiftCardRedeemData redeemData = (GiftCardRedeemData) GetRedeemData();
                if (diamond >= redeemData.diamond)
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
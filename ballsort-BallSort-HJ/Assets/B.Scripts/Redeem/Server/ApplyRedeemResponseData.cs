using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Redeem
{
    //[Serializable]
    //public class ApplyRedeemResponseData
    //{
    //    public bool success;
    //    public ApplyRedeemResponseDetailData redeem;
    //}

    [Serializable]
    public class ApplyRedeemResponseDetailData
    {
        public int id;
        public int user_id;
        public string redeem_date;
        public string confirm_date;
        public string audit_date;
        public string pay_date;
        public int amount;
        public string paypal;
        public string country;
        public int status;
        public string updated_at;
        public string created_at;
    }

    /**
       * 0: 已提交
       * 1: 已确认
       * 2: 已审核
       * 3: 已付款
       * 4: 已拒绝
       */
    public enum RedeemServerStatus
    {
        Applyed,
        Confirmed,
        Approved,
        Paid,
        Rejected,
    }
}

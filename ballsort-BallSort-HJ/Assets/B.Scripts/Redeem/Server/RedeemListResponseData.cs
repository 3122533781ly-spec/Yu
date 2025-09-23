using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Redeem
{
    [Serializable]
    public class RedeemListResponseData
    {
        public List<RedeemListResponseItemData> items;
    }

    [Serializable]
    public class RedeemListResponseItemData
    {
        public int id;
        public string redeem_date;
        public int amount;
        public string paypal;
        public int invite_users;
        public int active_users;
        public int status;
        public string note;
    }
}

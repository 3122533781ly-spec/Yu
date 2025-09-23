using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Redeem
{
    [Serializable]

    public class GiftCardRedeemData : RedeemConfigBaseData
    {
        public int diamond;
        public int cardID;
    }
}

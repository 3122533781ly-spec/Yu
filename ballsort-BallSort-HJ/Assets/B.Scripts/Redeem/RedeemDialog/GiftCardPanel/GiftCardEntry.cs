using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Redeem
{
    public class GiftCardEntry : RedeemBaseEntry
    {
        [SerializeField] private Image imgCard;

        public override void Show(RedeemEntryBaseData data)
        {
            base.Show(data);

            GiftCardInfoData giftCardInfoData = ((GiftCardEntryData)data).GetGiftCardInfoData();
            if (giftCardInfoData != null)
            {
                imgCard.sprite = giftCardInfoData.sprite;
            }

        }
    }
}

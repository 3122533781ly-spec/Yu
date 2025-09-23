using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Redeem
{
    public class CashOutPlayerHeadItem : MonoBehaviour
    {
        [SerializeField] private Image head;
        [SerializeField] private Text textMoney;

        public void Show(Sprite icon, int money)
        {
            head.sprite = icon;
            textMoney.text = $"${money}";
        }
    }
}

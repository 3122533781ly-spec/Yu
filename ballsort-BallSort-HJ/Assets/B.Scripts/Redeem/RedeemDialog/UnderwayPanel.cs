using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ToggleButton;

namespace Redeem
{
    public class UnderwayPanel : MonoBehaviour
    {
        [SerializeField] private Text textRewardProgress;
        [SerializeField] private Button btnRedeem;

        public enum HuoBType
        {
            Money,
            Diamond
        }

        [SerializeField] private HuoBType huobtype;
        private Action onRedeem;

        public void Init(Action redeemCallBack)
        {
            onRedeem = redeemCallBack;
        }

        public void Show(string progressStr, RedeemEntryState state)
        {
            btnRedeem.interactable = state == RedeemEntryState.CanApplyRedeem;
            textRewardProgress.text = progressStr;
        }

        private void OnEnable()
        {
            btnRedeem.onClick.AddListener(ClickRedeem);
            if (huobtype == HuoBType.Money)
                textRewardProgress.text = $"${Game.Instance.CurrencyModel.GetCurrentMoney()} / $100";
            else textRewardProgress.text = $"{Game.Instance.CurrencyModel.GetGoodNumber(GoodType.Gem, GoodSubType.Null)} / 10000";
        }

        private void OnDisable()
        {
            btnRedeem.onClick.RemoveListener(ClickRedeem);
        }

        private void ClickRedeem()
        {
            if (huobtype == HuoBType.Money)
            {
                string message = $"You have ${Game.Instance.CurrencyModel.GetCurrentMoney()}, only ${100 - Game.Instance.CurrencyModel.GetCurrentMoney()} left to go. Play more games to win more money.";

                FloatingWindow.Instance.Show(message);
            }
            else if (huobtype == HuoBType.Diamond)
            {
                string message = $"You have {Game.Instance.CurrencyModel.GetGoodNumber(GoodType.Gem, GoodSubType.Null)}, only {10000 - Game.Instance.CurrencyModel.GetGoodNumber(GoodType.Gem, GoodSubType.Null)} left to go Play more games to win more gems.";

                FloatingWindow.Instance.Show(message);
            }

            onRedeem?.Invoke();
        }
    }
}
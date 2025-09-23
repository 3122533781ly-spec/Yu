using System.Collections.Generic;
using ProjectSpace.Lei31Utils.Scripts.Framework.App;
using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;
using Redeem;
using UnityEngine;
using UnityEngine.UI;

namespace RedeemSpace.Scripts.Redeem.RedeemDialog
{
    public class RedeemDialog : Dialog
    {
        [SerializeField] private List<Text> moneyTexts;
        [SerializeField] private List<Text> diamondTexts;

        [SerializeField] private ToggleGroup toggleGroup;
        [SerializeField] private List<GameObject> panels;

        public override void ShowDialog()
        {
            base.ShowDialog();
            {
                Game.Instance.GetSystem<RedeemSystem>().CheckRedeem();
                Game.Instance.GetSystem<RedeemSystem>().HandleGetRedeemList(null);
                InitToggle(0);
                RefreshMoneyUI();
                RefreshDiamondUI();
            }
        }

        private void OnEnable()
        {
            toggleGroup.OnToggleChange += OnToggleChange;
            Game.Instance.CurrencyModel.RegisterMoneyChangeAction(HandleMoneyChanged);
            Game.Instance.CurrencyModel.Diamond.OnValueChange += HandleDiamondChanged;
        }

        private void OnDisable()
        {
            toggleGroup.OnToggleChange -= OnToggleChange;
            Game.Instance.CurrencyModel.UnregisterMoneyChangeAction(HandleMoneyChanged);
            Game.Instance.CurrencyModel.Diamond.OnValueChange -= HandleDiamondChanged;
        }

        private void InitToggle(int index)
        {
            toggleGroup.Init(index);
            OnToggleChange(index);
        }

        private void OnToggleChange(int index)
        {
            for (int i = 0; i < panels.Count; i++)
            {
                panels[i].SetActive(index == i);
            }
        }

        private void HandleMoneyChanged(float oldValue, float newValue)
        {
            RefreshMoneyUI();
        }

        private void RefreshMoneyUI()
        {
            foreach (var item in moneyTexts)
            {
                item.text = $"${Game.Instance.CurrencyModel.GetCurrentMoney():f2}";
            }
        }

        private void HandleDiamondChanged(int oldValue, int newValue)
        {
            RefreshDiamondUI();
        }

        private void RefreshDiamondUI()
        {
            foreach (var item in diamondTexts)
            {
                item.text = Game.Instance.CurrencyModel.Diamond.Value.ToString();
            }
        }
    }
}

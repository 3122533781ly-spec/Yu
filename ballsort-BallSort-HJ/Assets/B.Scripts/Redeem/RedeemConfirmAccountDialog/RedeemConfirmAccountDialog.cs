using Redeem;
using System;
using ProjectSpace.Lei31Utils.Scripts.Framework.App;
using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;
using UnityEngine;
using UnityEngine.UI;

public class RedeemConfirmAccountDialog : Dialog
{
    [SerializeField] private Text textReward;
    [SerializeField] private Text textAccount;
    [SerializeField] private Button btnChange;
    [SerializeField] private Button btnSure;

    private Action onChange;
    private Action onSure;

    public void Show(int moneyCount, Action changeCallBack, Action sureCallBack)
    {
        onChange = changeCallBack;
        onSure = sureCallBack;
        textReward.text = $"${moneyCount}";
        textAccount.text = Game.Instance.GetSystem<RedeemSystem>().Model.paypalAccount;

        Activate();
    }

    private void OnEnable()
    {
        btnChange.onClick.AddListener(ClickChange);
        btnSure.onClick.AddListener(ClickSure);
    }

    private void OnDisable()
    {
        btnChange.onClick.RemoveListener(ClickChange);
        btnSure.onClick.RemoveListener(ClickSure);
    }

    private void ClickChange()
    {
        Deactivate();
        onChange?.Invoke();
    }

    private void ClickSure()
    {
        Deactivate();
        onSure?.Invoke();
    }
}

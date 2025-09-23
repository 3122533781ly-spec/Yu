using Redeem;
using System;
using ProjectSpace.Lei31Utils.Scripts.Framework.App;
using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;
using UnityEngine;
using UnityEngine.UI;

public class RedeemAccountDialog : Dialog
{
    [SerializeField] private Button btnSubmit;
    [SerializeField] private InputField inputField;

    private Action onFinishInputAccount;

    public void Show(Action callBack = null)
    {
        onFinishInputAccount = callBack;
        if (!string.IsNullOrEmpty(Game.Instance.GetSystem<RedeemSystem>().Model.paypalAccount))
        {
            inputField.text = Game.Instance.GetSystem<RedeemSystem>().Model.paypalAccount;
        }

        Activate();
    }

    private void OnEnable()
    {
        btnSubmit.onClick.AddListener(ClickSubmit);
    }

    private void OnDisable()
    {
        btnSubmit.onClick.RemoveListener(ClickSubmit);
    }

    private void ClickSubmit()
    {
        if (!string.IsNullOrEmpty(inputField.text))
        {
            Game.Instance.GetSystem<RedeemSystem>().ChangePayPalAccount(inputField.text);
            Deactivate();
            onFinishInputAccount?.Invoke();
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;

public class DiamondGetDialog : Dialog
{
    private void OnEnable()
    {
        _btnAConfirm.onClick.AddListener(CloseDialog);
        _btnClose.onClick.AddListener(CloseDialog);
        // Refresh();
    }

    private void OnDisable()
    {
        //    _btnADClaim.onClick.RemoveListener(ClickUseAD);
    }

    private void CloseDia()
    {
        CloseDialog();
    }

    [SerializeField] private Button _btnAConfirm;
    [SerializeField] private Button _btnClose;
}
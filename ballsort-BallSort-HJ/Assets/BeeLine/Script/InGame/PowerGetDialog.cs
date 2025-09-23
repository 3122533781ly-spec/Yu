using UnityEngine;
using UnityEngine.UI;
using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;

public class PowerGetDialog : Dialog
{
    private void OnEnable()
    {
        _btnADClaim.onClick.AddListener(ClickUseAD);
        _btnClose.onClick.AddListener(CloseDialog);
    }

    private void OnDisable()
    {
        _btnADClaim.onClick.RemoveListener(ClickUseAD);
    }

    private void CloseDia()
    {
        CloseDialog();
    }

    private void ClickUseAD()
    {
        ADMudule.ShowRewardedAd(ADPosConst.GetCoinDialog, (isSuccess) =>
        {
            if (isSuccess)
            {
                //IStaticDelegate.SourceCurrency("Coin", GameConfig.Instance.AdRewardCoin, "AD",
                //    ADPosConst.GetCoinInGame);
                LineBee.Instance.PowerSystem.RewardGamePower(20);
                CloseDialog();
            }
        });
    }

    [SerializeField] private Button _btnADClaim;
    [SerializeField] private Button _btnClose;
}
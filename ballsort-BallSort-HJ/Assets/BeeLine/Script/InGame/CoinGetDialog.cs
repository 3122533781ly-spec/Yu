using UnityEngine;
using UnityEngine.UI;
using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;

public class CoinGetDialog : Dialog
{
    private void OnEnable()
    {
        _btnADClaim.onClick.AddListener(ClickUseAD);
        _btnClose.onClick.AddListener(CloseDialog);
        Refresh();
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
        if (Game.Instance.CurrencyModel.DiamondNum < 1)
        {
            FloatingWindow.Instance.Show("Not enough tickets");
            return;
        }
        Game.Instance.CurrencyModel.ConsumeDiamond(1);
        TransitionManager.Instance.Transition(0.5f, () => { SceneManager.LoadScene("InGameScenario"); },
                     0.5f);
        CloseDialog();
    }

    private void Refresh()
    {
        _textRewardCoinNum.text = $"{Game.Instance.CurrencyModel.DiamondNum}/1";
    }

    [SerializeField] private Text _textRewardCoinNum;
    [SerializeField] private Text _textLevel;
    [SerializeField] private Button _btnADClaim;
    [SerializeField] private Button _btnClose;
}
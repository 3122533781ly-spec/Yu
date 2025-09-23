using ProjectSpace.Lei31Utils.Scripts.Framework.App;
using UnityEngine;

public class DiamondDisplayBar : MonoBehaviour
{
    private void OnEnable()
    {
        _displayer.ResetNumber(Game.Instance.CurrencyModel.DiamondNum);
        Game.Instance.CurrencyModel.RegisterDiamondChangeAction(OnDiamondChange);
    }

    private void OnDisable()
    {
        Game.Instance.CurrencyModel.UnregisterDiamondChangeAction(OnDiamondChange);
    }

    private void OnDiamondChange(int oldValue, int newValue)
    {
        if (_unit != null && newValue > oldValue)
        {
            int flyNum = Mathf.Clamp(newValue - oldValue, 1, 20);

            _unit.Play(flyNum, () =>
            {
                if (_displayer != null)
                {
                    _displayer.Number = newValue;
                }
            });
        }
        else
        {
            _displayer.Number = newValue;
        }
    }

    [SerializeField] private IntNumberDisplayer _displayer = null;
    [SerializeField] private CoinFlyUnit _unit;
}
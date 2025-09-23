using ProjectSpace.Lei31Utils.Scripts.Framework.App;
using UnityEngine;

public class CoinDisplayBar : MonoBehaviour
{
    private void OnEnable()
    {
        _displayer.ResetNumber(Game.Instance.CurrencyModel.CoinNum);
        Game.Instance.CurrencyModel.RegisterCoinChangeAction(OnCoinChange);
    }

    private void OnDisable()
    {
        Game.Instance.CurrencyModel.UnregisterCoinChangeAction(OnCoinChange);
    }

    private void OnCoinChange(int oldValue, int newValue)
    {
        if (_unit != null && newValue > oldValue)
        {
            int flyNum = Mathf.Clamp(newValue - oldValue, 1, 20);

            _unit.Play( flyNum, () =>
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
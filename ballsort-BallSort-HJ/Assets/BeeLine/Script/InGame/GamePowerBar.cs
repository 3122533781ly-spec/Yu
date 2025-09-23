using UnityEngine;
using UnityEngine.UI;

public class GamePowerBar : MonoBehaviour
{
    private void OnEnable()
    {
        Refresh();
        LineBee.Instance.PowerSystem.GamePower.OnValueChange += OnValueChange;
    }

    private void OnDisable()
    {
        LineBee.Instance.PowerSystem.GamePower.OnValueChange -= OnValueChange;
    }

    private void OnValueChange(int oldValue, int newValue)
    {
        Refresh();
    }

    private void Refresh()
    {
        // _fullTextObj.SetActive(LineBee.Instance.GetSystem<GamePowerSystem>().IsFull);
        //  _recoverTextObj.SetActive(!LineBee.Instance.GetSystem<GamePowerSystem>().IsFull);
        _textNumber.ResetNumber(LineBee.Instance.PowerSystem.GamePower.Value);

        _remainRecoverTime.text =
            DataFormater.ToTimeString(LineBee.Instance.PowerSystem.RemainRecoverTime);
    }

    private void Update()
    {
        _remainRecoverTime.text =
            DataFormater.ToTimeString(LineBee.Instance.PowerSystem.RemainRecoverTime);
        //_unlimitedTimeTime.text =
        //    DataFormater.ToTimeString(LineBee.Instance.PowerSystem.UnlimitedTime.Value);

        //_unLimitState.SetActive(LineBee.Instance.GetSystem<GamePowerSystem>().IsInfiniteState());
        //_timeState.SetActive(!LineBee.Instance.GetSystem<GamePowerSystem>().IsInfiniteState());
    }

    [SerializeField] private Text _remainRecoverTime;
    [SerializeField] private IntNumberDisplayer _textNumber;
    [SerializeField] private GameObject _fullTextObj;
    [SerializeField] private GameObject _recoverTextObj;

    [SerializeField] private Text _unlimitedTimeTime;
    [SerializeField] private GameObject _unLimitState;
    [SerializeField] private GameObject _timeState;
}
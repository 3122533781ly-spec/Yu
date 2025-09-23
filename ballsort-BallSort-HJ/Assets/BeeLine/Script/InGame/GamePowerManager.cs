using System;
using UnityEngine;

public class GamePowerManager : MonoBehaviour
{
    private void UpdatePowerRecover()
    {
        int startRecover = LineBee.Instance.PowerSystem.StartRecoverTime;
        int now = DataFormater.ConvertDateTimeToTimeStamp(DateTime.Now);
        int duration = now - startRecover;
        if (duration > GamePowerSystem.RecoverSecond)
        {
            int recoverPoint = duration / GamePowerSystem.RecoverSecond;
            LineBee.Instance.PowerSystem.Recover(recoverPoint);
            int passTime = recoverPoint * GamePowerSystem.RecoverSecond;

            int newStartRecoverTime = startRecover + passTime;
            LineBee.Instance.PowerSystem.SetStartRecoverTime(newStartRecoverTime);
        }
    }

    private void Update()
    {
        if (LineBee.Instance.PowerSystem.GamePower.Value < GamePowerSystem.GamePower_MAX)
        {
            UpdatePowerRecover();
        }
    }
}
using System;
using UnityEngine;

public class GamePowerSystem
{
    public PersistenceData<int> GamePower { get; private set; }

    public PersistenceData<int> VIPLevel { get; private set; }

    public bool IsVIPTop => VIPLevel.Value >= 3;

    public int StartRecoverTime => _startRecoverTime.Value;

    //无限体力时间
    public PersistenceData<int> UnlimitedTime { get; private set; }

    public const int GamePower_Default = 15;
    public const int GamePower_MAX = 50;
    public const int GamePower_Min = 0;
    public const int RecoverSecond = 60;

    public int RemainRecoverTime
    {
        get
        {
            int start = _startRecoverTime.Value;
            int now = DataFormater.ConvertDateTimeToTimeStamp(DateTime.Now);

            return Mathf.Max(0, RecoverSecond - (now - start));
        }
    }

    public void VIPLevelUP()
    {
        VIPLevel.Value += 1;

        //Claim
        if (VIPLevel.Value == 1)
        {
            //10 power
            RewardGamePower(10);
        }
        else if (VIPLevel.Value == 2)
        {
            //10 power
            RewardGamePower(20);
        }
        else if (VIPLevel.Value == 3)
        {
            AddUnlimitedTime(60 * 24 * 60);
        }
    }

    public void AddUnlimitedTime(int second)
    {
        UnlimitedTime.Value = Mathf.Max(0, UnlimitedTime.Value + second);
    }

    public void ConsumeUnlimitedTime(int second)
    {
        UnlimitedTime.Value = Mathf.Max(0, UnlimitedTime.Value - second);
    }

    public bool IsInfiniteState()
    {
        return UnlimitedTime.Value > 0;
    }

    public bool CanConsume()
    {
        if (UnlimitedTime.Value > 0)
        {
            return true;
        }

        return GamePower.Value >= 5;
    }

    public bool IsFull => GamePower.Value >= GamePower_MAX;

    /// <summary>
    /// 消耗 5 体力
    /// </summary>
    public void ConsumeGamePower()
    {
        if (UnlimitedTime.Value <= 0)
        {
            int old = GamePower.Value;
            GamePower.Value = Mathf.Max(GamePower.Value - 5, 0);

            if (old == GamePower_MAX)
            {
                _startRecoverTime.Value = DataFormater.ConvertDateTimeToTimeStamp(DateTime.Now);
            }
        }
    }

    /// <summary>
    /// 奖励体力
    /// </summary>
    /// <param name="value"></param>
    public void RewardGamePower(int value)
    {
        GamePower.Value = Mathf.Clamp(GamePower.Value + value, GamePower_Min, 999);
    }

    /// <summary>
    /// 恢复体力
    /// </summary>
    /// <param name="value"></param>
    public void Recover(int value)
    {
        GamePower.Value = Mathf.Clamp(GamePower.Value + value, GamePower_Min, GamePower_MAX);
    }

    public void SetStartRecoverTime(int value)
    {
        _startRecoverTime.Value = value;
    }

    public GamePowerSystem()
    {
        GamePower = new PersistenceData<int>("GamePowerSystem_GamePower", GamePower_Default);
        UnlimitedTime = new PersistenceData<int>("GamePowerSystem_UnlimitedTime", 0);
        _startRecoverTime = new PersistenceData<int>("GamePowerSystem_startRecoverTime",
            DataFormater.ConvertDateTimeToTimeStamp(DateTime.Now));

        int dayTimestamp = DataFormater.GetDayTimeStamp(DateTime.Now);
        VIPLevel = new PersistenceData<int>($"GamePowerSystem_VIPLevel_{dayTimestamp}", 0);
    }

    //开始恢复体力的时间点
    private PersistenceData<int> _startRecoverTime;
}
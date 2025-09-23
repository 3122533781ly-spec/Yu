using System;
using System.Collections.Generic;
using UnityEngine;

public class LineBeeLevelModel
{
    public int EnterLevelID;
    public bool HasHint { get; private set; }
    public LevelMode EnterMode { get; set; }

    public Action<int> OnChangeScore = delegate (int i) { };
    public Action<int> OnChangeCombo = delegate (int i) { };

    public int Score
    {
        get => _score;
        set
        {
            if (_score != value)
            {
                _score = value;
                OnChangeScore.Invoke(value);
            }
        }
    }

    public void SetHasHint(bool TF)
    {
        HasHint = TF;
    }

    public int Combo
    {
        get => _combo;
        set
        {
            if (_combo != value)
            {
                _combo = value;
                OnChangeCombo.Invoke(value);
            }
        }
    }

    public int Collider { get; set; }

    public PersistenceData<int> MaxUnlockLevel;

    //当天打过的关卡数
    public PersistenceData<int> TodayHasPlayedLevel;

    public bool HaveReviveUseAD { get; set; }

    public void PlusTodayLevelCount()
    {
        TodayHasPlayedLevel.Value++;
    }

    public void ResetReviveUseAD()
    {
        HaveReviveUseAD = false;
    }

    public void PassLevel()
    {
        MaxUnlockLevel.Value++;
    }

    public void SetEnterLevelID(int value)
    {
        EnterLevelID = value;
    }

    public void PassCurrentLevel()
    {
        //int nextLevel = EnterLevelID + 1;
        //Debug.Log("当前关卡ID+" + MaxUnlockLevel.Value);
        //if (MaxUnlockLevel.Value <= nextLevel)
        //{
        MaxUnlockLevel.Value++;
        //}
    }

    public LineBeeLevelModel()
    {
        int dayTimeStamp = DataFormater.GetDayTimeStamp(DateTime.Now);
        TodayHasPlayedLevel = new PersistenceData<int>($"AppModel_tol_{dayTimeStamp}", 0);
        HaveReviveUseAD = false;
        MaxUnlockLevel = new PersistenceData<int>("LineBeeLevelModel_MaxUnlockLevel", 1);
    }

    private int _score;
    private int _combo;
}

public enum LevelMode
{
    LinkFlip,
    LinkTwo,
}
using System;

public class CheckinSystem : GameSystem
{
    public int CurrentCheckinDayIndex { get; private set; }

    public void PlusCheckinDay()
    {
        CurrentCheckinDayIndex++;
        if (CurrentCheckinDayIndex == 6)
        {
            //Completed
            CurrentCheckinDayIndex = -1;
        }

        Storage.Instance.SetInt(CurrentCheckinDayIndexKey, CurrentCheckinDayIndex);
        LastShowTime = DataFormater.ConvertDateTimeToTimeStamp(DateTime.Now);
        HasCheckInToday = true;
    }

    public bool NeedShowToday()
    {
        //int today = DataFormater.GetDayTimeStamp(DateTime.Now);
        //return LastShowTime < today;
        return !HasCheckInToday;
    }

    public override void Init()
    {
        CurrentCheckinDayIndex = Storage.Instance.GetInt(CurrentCheckinDayIndexKey, -1);
    }

    public override void Destroy()
    {
    }

    public bool HasCheckInToday
    {
        get
        {
            string strKey = DateTime.Today.ToString("yyyy_MM_dd_checkin");
            return Storage.Instance.GetBool(strKey);
        }
        set
        {
            string strKey = DateTime.Today.ToString("yyyy_MM_dd_checkin");
            Storage.Instance.SetBool(strKey, value);
        }
    }
    public int LastShowTime
    {
        get { return Storage.Instance.GetInt(LastShowTimeKey, 0); }
        set { Storage.Instance.SetInt(LastShowTimeKey, value); }
    }

    private const string CurrentCheckinDayIndexKey = "CurrentCheckinDayIndexKey";
    private const string LastShowTimeKey = "LastShowTimeKey";
}
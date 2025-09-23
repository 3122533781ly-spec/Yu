using System;
using ProjectSpace.Lei31Utils.Scripts.Framework.App;

public class UserPurchaseHabitSystem : GameSystem
{
    public PersistenceData<int> CurrentPurchaseType;
    public PurchaseUser Habit;

    public UserPurchaseType CurrentType
    {
        get { return (UserPurchaseType) CurrentPurchaseType.Value; }
        set { CurrentPurchaseType.Value = (int) value; }
    }

    public void UserIAPHappen()
    {
        CurrentType = UserPurchaseType.HighInAppPurchase;
        StaticModule.PurchaseHabitToHighInAppPurchase();
    }

    public void WatchRewardPlus()
    {
        _accRewardADTime.Value++;
        if (_accRewardADTime.Value == 4)
        {
            //累计3天激励大于4
            CurrentType = UserPurchaseType.HighAD;
            StaticModule.PurchaseHabitToHighAD();
        }
    }

    public bool HaveTripleTime()
    {
        return Game.Instance.Model.WatchTripleCoinTime.Value < Habit.TripleCoinADTime;
    }

    public override void Init()
    {
        CurrentPurchaseType =
            new PersistenceData<int>("UserPurchaseHabitSystem_CurrentPurchaseType", (int) UserPurchaseType.Guest);
        Habit = new PurchaseUser(CurrentType);


        _accRewardADTime =
            new PersistenceData<int>(
                $"UserPurchaseHabitSystem_accRewardADTime{DataFormater.GetDayTimeStamp(DateTime.Now)}", 0);
    }

    public override void Destroy()
    {
    }

    //累积的当天激励广告数量,每天刷新
    private PersistenceData<int> _accRewardADTime;
}
using System.Text;

public class PurchaseUser
{
    public UserPurchaseType Type;

    //正常模式复活次数
    public int NormalLevelReviveTime;

    //每日挑战复活次数
    public int DailyChallengeReviveTime;

    //三倍金币
    public int TripleCoinADTime;

    //钱袋子 多少关出现一次
    public int WalletTimePreLevel;

    public PurchaseUser(UserPurchaseType type)
    {
        Type = type;
        Init();
    }

    private void Init()
    {
        switch (Type)
        {
            case UserPurchaseType.Guest:
                NormalLevelReviveTime = 3;
                DailyChallengeReviveTime = 6;
                TripleCoinADTime = 3;
                WalletTimePreLevel = 3;
                break;
            case UserPurchaseType.HighAD:
                NormalLevelReviveTime = 6;
                DailyChallengeReviveTime = 99;
                TripleCoinADTime = 5;
                WalletTimePreLevel = 1;
                break;
            case UserPurchaseType.HighInAppPurchase:
                NormalLevelReviveTime = 3;
                DailyChallengeReviveTime = 3;
                TripleCoinADTime = 1;
                WalletTimePreLevel = 5;
                break;
        }
    }

    public override string ToString()
    {
        StringBuilder builer = new StringBuilder();
        builer.Append("NormalLevelReviveTime:" + NormalLevelReviveTime);
        builer.Append("\n");
        builer.Append("DailyChallengeReviveTime:" + DailyChallengeReviveTime);
        builer.Append("\n");
        builer.Append("TripleCoinADTime:" + TripleCoinADTime);
        builer.Append("\n");
        builer.Append("WalletTimePreLevel:" + WalletTimePreLevel);
        builer.Append("\n");
        return builer.ToString();
    }
}
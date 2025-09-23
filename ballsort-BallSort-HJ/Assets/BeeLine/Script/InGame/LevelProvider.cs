using _02.Scripts.LevelEdit;

public class LevelProvider
{
    public static LineBeeLevelData GetInGameLevelData(int enterGameLevelId)
    {
        return GetNormalLevelData(enterGameLevelId);
    }

    public static LineBeeLevelData GetNormalLevelData(int enterGameLevelId)
    {
        if (LevelDataConfig.Instance.TryGetConfigByID(enterGameLevelId, out LineBeeLevelData fixedLevel))
        {
            LDebug.Log("LevelProvider", "Current level is fixedLevel");
            return fixedLevel;
        }
        else
        {
            LevelDataConfig.Instance.TryGetConfigByID(1,
                out LineBeeLevelData level);
            return level;
        }
    }
}
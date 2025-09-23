using Fangtang;

public class CheckTagDelegate : ICheckTagDelegate
{
    public bool Check(int tagInt)
    {
#if UNITY_EDITOR
        return LogTagStore.Instance.Check(tagInt);
#else
        return (Fangtang.LogTag.RuntimeFlagInt & (1 << (tagInt - 1))) != 0;
#endif
    }

    public string GetTagFormatString(int tagInt, string value)
    {
#if UNITY_EDITOR
        return string.Format("<color=yellow>{0}({2})</color>: {1}", LogTag.GetTag(tagInt), value, 
            UnityEngine.Time.frameCount);
#else
        return string.Format("{0}({2}): {1}", LogTag.GetTag(tagInt), value, UnityEngine.Time.frameCount);
#endif
    }
}

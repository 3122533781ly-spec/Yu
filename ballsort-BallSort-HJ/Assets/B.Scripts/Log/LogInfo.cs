using UnityEngine;

public struct LogInfo
{
    public string Id;
    public string Content;
    public string StackTrace;
    public LogType Type;

    public static bool operator ==(LogInfo a, LogInfo b)
    {
        return a.Id == b.Id;
    }

    public static bool operator !=(LogInfo a, LogInfo b)
    {
        return a.Id != b.Id;
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
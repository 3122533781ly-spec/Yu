using UnityEngine;

[System.Serializable]
public class DotPointData
{
    public Vector2Int Cordinates;
    public int Value;

    public bool IsBee => Value == 2;

    //public DotPointFaceData GetFaceData()
    //{
    //    DotPointFaceData data = DotPointFaceConfig.Instance.GetConfigByID(Value);
    //    return data;
    //}
}
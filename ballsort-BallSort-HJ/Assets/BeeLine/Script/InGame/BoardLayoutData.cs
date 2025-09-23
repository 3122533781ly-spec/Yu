using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class BoardLayoutData : SerializedScriptableObject
{
    public List<DotPointData> PointDatas;

    public static BoardLayoutData GetBoardDataByID(int id)
    {
        BoardLayoutData result = Resources.Load<BoardLayoutData>(GetBoardPath(id));
        if (result == null)
        {
            result = Resources.Load<BoardLayoutData>(GetBoardPath(1));
        }

        return result;
    }

    public static string GetBoardPath(int id)
    {
        return $"BoardLayouts/{id}";
    }

    public static void CheckValid(int id)
    {
        BoardLayoutData layout = BoardLayoutData.GetBoardDataByID(id);

        if (layout == null)
        {
            Debug.LogError($"造型id 不存在:{id}");
        }
    }
}
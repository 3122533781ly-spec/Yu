using System.Collections.Generic;
using UnityEngine;

public static class BoardMapHelper
{
    public static Vector3 GetOffsetByTileModes()
    {
        return Vector3.zero;
    }

    public static bool HaveNeighbor(this List<DotPoint> origin)
    {
        for (int i = 0; i < origin.Count; i++)
        {
            for (int j = 0; j < origin.Count; j++)
            {
                if (i == j)
                    continue;

                if (IsNeighbor(origin[i].Data.Cordinates, origin[j].Data.Cordinates))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool IsNeighbor(DotPoint one, DotPoint two)
    {
        return IsNeighbor(one.Data.Cordinates, two.Data.Cordinates);
    }

    public static bool IsNeighbor(Vector2Int coordinate1, Vector2Int coordinate2)
    {
        return (Mathf.Abs(coordinate1.x - coordinate2.x) == 1 && coordinate1.y == coordinate2.y) ||
               (Mathf.Abs(coordinate1.y - coordinate2.y) == 1 && coordinate1.x == coordinate2.x);
    }
    
    
}
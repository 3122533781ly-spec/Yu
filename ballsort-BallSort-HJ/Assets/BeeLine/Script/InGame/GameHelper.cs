using System;
using System.Collections.Generic;
using UnityEngine;

public class GameHelper
{
    public static Vector3 GetPointLocalPosition(Vector2Int cordinates)
    {
        float x = cordinates.x * PointConst.PointCordinatesUnit;
        float y = -cordinates.y * PointConst.PointCordinatesUnit;

        return new Vector3(x, y, 0);
    }

    public static int GetAllBeeCount(List<DotPoint> candidate)
    {
        int result = 0;

        for (int i = 0; i < candidate.Count; i++)
        {
            if (candidate[i].Data.IsBee)
            {
                result++;
            }
        }

        return result;
    }

    //根据滑动方向趋势，获取目标点
    public static DotPoint GetDirPoint(List<DotPoint> candidate, DotPoint center, Vector2 direction)
    {
        if (Math.Abs(Mathf.Abs(direction.x) - Mathf.Abs(direction.y)) < 0.01f)
        {
            return null;
        }

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            //右
            if (direction.x > 0)
            {
                return GetByCordinates(candidate,
                    new Vector2Int(center.Data.Cordinates.x + 1, center.Data.Cordinates.y));
            }
            //左
            else
            {
                return GetByCordinates(candidate,
                    new Vector2Int(center.Data.Cordinates.x - 1, center.Data.Cordinates.y));
            }
        }
        else
        {
            //上
            if (direction.y > 0)
            {
                return GetByCordinates(candidate,
                    new Vector2Int(center.Data.Cordinates.x, center.Data.Cordinates.y - 1));
            }
            //下
            else
            {
                return GetByCordinates(candidate,
                    new Vector2Int(center.Data.Cordinates.x, center.Data.Cordinates.y + 1));
            }
        }
    }

    public static DotPoint GetByCordinates(List<DotPoint> candidate, Vector2Int cordinates)
    {
        for (int i = 0; i < candidate.Count; i++)
        {
            if (candidate[i].Data.Cordinates == cordinates)
            {
                return candidate[i];
            }
        }

        return null;
    }

    public static List<DotPoint> GetNeighbor(List<DotPoint> all, DotPoint target)
    {
        List<DotPoint> result = new List<DotPoint>();

        DotPoint up = GetByCordinates(all, target.Data.Cordinates + Vector2Int.up);
        if (up != null)
            result.Add(up);
        DotPoint down = GetByCordinates(all, target.Data.Cordinates + Vector2Int.down);
        if (down != null)
            result.Add(down);
        DotPoint left = GetByCordinates(all, target.Data.Cordinates + Vector2Int.left);
        if (left != null)
            result.Add(left);
        DotPoint right = GetByCordinates(all, target.Data.Cordinates + Vector2Int.right);
        if (right != null)
            result.Add(right);
        return result;
    }
}
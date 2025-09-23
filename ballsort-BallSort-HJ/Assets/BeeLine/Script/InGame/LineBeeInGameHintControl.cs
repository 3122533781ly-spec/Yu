using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Fangtang;
using Sirenix.OdinInspector;

public class LineBeeInGameHintControl : ElementBehavior<InGameLineBee>
{
    [Header("提示配置")]
    [SerializeField] private GameObject pathObjectPrefab; // 路径点预制体

    [SerializeField] private Transform pathParent;        // 路径物体父级
    [SerializeField] private float objectScaleTime = 0.3f; // 物体缩放动画时间
    [SerializeField] private Ease objectScaleEase = Ease.OutBounce; // 缩放动画曲线

    [Header("屏幕坐标转换配置")]
    [SerializeField] private Camera sourceCamera; // 源摄像机（路径点所在摄像机）

    [SerializeField] private Camera targetCamera; // 目标摄像机（生成物体所在摄像机）
    [SerializeField] private float defaultZDepth = 10f; // 默认Z轴深度（透视摄像机）

    private List<GameObject> spawnedPathObjects = new List<GameObject>(); // 已生成的物体

    [Button]
    public void ShowHint()
    {
        List<DotPoint> path = new List<DotPoint>();
        if (FindMovePath(path))
        {
            // 显示提示线
            Context.GetView<InGameLineView>().PlayViewHint(path);

            // 生成路径物体（使用屏幕相对位置转换）
            SpawnObjectsWithScreenPosition(path);
        }
        else
        {
            Debug.LogError("Error find hint move path.");
        }
    }

    [Button]
    public void ClearHint()
    {
        ClearSpawnedObjects();
        //    Context.GetView<InGameLineView>().ClearHint();
    }

    private bool FindMovePath(List<DotPoint> result)
    {
        int allBeeCount = GameHelper.GetAllBeeCount(Context.MatchModel.AllPoint);
        for (int i = 0; i < Context.MatchModel.AllPoint.Count; i++)
        {
            DotPoint point = Context.MatchModel.AllPoint[i];

            if (point.Data.IsBee)
            {
                List<DotPoint> movePath = new List<DotPoint>() { point };
                CheckMovePath(movePath, allBeeCount);

                if (movePath.Count == allBeeCount)
                {
                    result.AddRange(movePath);
                    return true;
                }
            }
        }
        return false;
    }

    public void CheckMovePath(List<DotPoint> selectedQuene, int allBeeCount)
    {
        DotPoint last = selectedQuene[selectedQuene.Count - 1];

        List<Vector2> moveDir = new List<Vector2>()
        {
            Vector2.left,
            Vector2.right,
            Vector2.up,
            Vector2.down
        };

        for (int i = 0; i < moveDir.Count; i++)
        {
            DotPoint findPoint = GameHelper.GetDirPoint(Context.MatchModel.AllPoint, last, moveDir[i]);

            if (findPoint == null || selectedQuene.Contains(findPoint) || !findPoint.Data.IsBee)
                continue;

            selectedQuene.Add(findPoint);
            CheckMovePath(selectedQuene, allBeeCount);

            if (selectedQuene.Count == allBeeCount)
            {
                return;
            }
            else
            {
                selectedQuene.Remove(findPoint);
            }
        }
    }

    private void SpawnObjectsWithScreenPosition(List<DotPoint> path)
    {
        ClearSpawnedObjects();

        for (int i = 0; i < path.Count; i++)
        {
            // 1. 获取源摄像机中的世界坐标
            Vector3 sourceWorldPos = path[i].Transform.position;
            sourceWorldPos.z = 0; // 2D游戏Z轴设为0

            // 2. 转换为源摄像机的屏幕坐标 (x,y,z为深度)
            Vector3 sourceScreenPos = sourceCamera.WorldToScreenPoint(sourceWorldPos);

            // 3. 标准化屏幕坐标 (0-1范围)
            Vector2 normalizedScreenPos = new Vector2(
                sourceScreenPos.x / Screen.width,
                sourceScreenPos.y / Screen.height
            );

            // 4. 转换为目标摄像机的世界坐标
            Vector3 targetWorldPos = ConvertNormalizedScreenToWorld(normalizedScreenPos, targetCamera) - new Vector3(540, 960, 950);

            // 生成物体
            GameObject obj = Instantiate(pathObjectPrefab, targetWorldPos, Quaternion.identity);
            obj.transform.SetParent(pathParent, false);
            obj.GetComponent<HIntText>().Refresh(i + 1);
            spawnedPathObjects.Add(obj);

            // 添加生成动画
            obj.transform.localScale = Vector3.zero;
            obj.transform.DOScale(Vector3.one, objectScaleTime).SetEase(objectScaleEase);

            // 设置颜色
            SetPathObjectColor(obj, i, path.Count);

            // 调试输出
            Debug.Log($"源世界坐标: {sourceWorldPos}, 源屏幕坐标: {sourceScreenPos}, " +
                      $"标准化坐标: {normalizedScreenPos}, 目标世界坐标: {targetWorldPos}");
        }
    }

    /// <summary>
    /// 将标准化屏幕坐标转换为目标摄像机的世界坐标
    /// </summary>
    private Vector3 ConvertNormalizedScreenToWorld(Vector2 normalizedScreenPos, Camera targetCamera)
    {
        // 转换为屏幕像素坐标
        float screenX = normalizedScreenPos.x * Screen.width;
        float screenY = normalizedScreenPos.y * Screen.height;

        // 构建屏幕坐标（Z值根据摄像机类型设置）
        Vector3 screenPos = new Vector3(screenX, screenY, 0);

        if (targetCamera.orthographic)
        {
            // 正交摄像机：Z轴设为相机的正交大小的一半
            screenPos.z = targetCamera.orthographicSize;
        }
        else
        {
            // 透视摄像机：Z轴设为默认深度
            screenPos.z = defaultZDepth;
        }

        // 转换为世界坐标
        return targetCamera.ScreenToWorldPoint(screenPos);
    }

    private void SetPathObjectColor(GameObject obj, int index, int totalCount)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer == null) return;

        if (index == 0) // 起点
        {
            renderer.material.color = Color.green;
        }
        else if (index == totalCount - 1) // 终点
        {
            renderer.material.color = Color.red;
        }
        else // 中间点
        {
            float t = (float)index / (totalCount - 1);
            renderer.material.color = Color.Lerp(Color.blue, Color.yellow, t);
        }
    }

    private void ClearSpawnedObjects()
    {
        foreach (var obj in spawnedPathObjects)
        {
            if (obj != null)
                Destroy(obj);
        }
        spawnedPathObjects.Clear();
    }
}
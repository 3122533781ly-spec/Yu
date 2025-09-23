using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Fangtang;
using Sirenix.OdinInspector;

public class LineBeeInGameHintControl : ElementBehavior<InGameLineBee>
{
    [Header("��ʾ����")]
    [SerializeField] private GameObject pathObjectPrefab; // ·����Ԥ����

    [SerializeField] private Transform pathParent;        // ·�����常��
    [SerializeField] private float objectScaleTime = 0.3f; // �������Ŷ���ʱ��
    [SerializeField] private Ease objectScaleEase = Ease.OutBounce; // ���Ŷ�������

    [Header("��Ļ����ת������")]
    [SerializeField] private Camera sourceCamera; // Դ�������·���������������

    [SerializeField] private Camera targetCamera; // Ŀ����������������������������
    [SerializeField] private float defaultZDepth = 10f; // Ĭ��Z����ȣ�͸���������

    private List<GameObject> spawnedPathObjects = new List<GameObject>(); // �����ɵ�����

    [Button]
    public void ShowHint()
    {
        List<DotPoint> path = new List<DotPoint>();
        if (FindMovePath(path))
        {
            // ��ʾ��ʾ��
            Context.GetView<InGameLineView>().PlayViewHint(path);

            // ����·�����壨ʹ����Ļ���λ��ת����
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
            // 1. ��ȡԴ������е���������
            Vector3 sourceWorldPos = path[i].Transform.position;
            sourceWorldPos.z = 0; // 2D��ϷZ����Ϊ0

            // 2. ת��ΪԴ���������Ļ���� (x,y,zΪ���)
            Vector3 sourceScreenPos = sourceCamera.WorldToScreenPoint(sourceWorldPos);

            // 3. ��׼����Ļ���� (0-1��Χ)
            Vector2 normalizedScreenPos = new Vector2(
                sourceScreenPos.x / Screen.width,
                sourceScreenPos.y / Screen.height
            );

            // 4. ת��ΪĿ�����������������
            Vector3 targetWorldPos = ConvertNormalizedScreenToWorld(normalizedScreenPos, targetCamera) - new Vector3(540, 960, 950);

            // ��������
            GameObject obj = Instantiate(pathObjectPrefab, targetWorldPos, Quaternion.identity);
            obj.transform.SetParent(pathParent, false);
            obj.GetComponent<HIntText>().Refresh(i + 1);
            spawnedPathObjects.Add(obj);

            // ������ɶ���
            obj.transform.localScale = Vector3.zero;
            obj.transform.DOScale(Vector3.one, objectScaleTime).SetEase(objectScaleEase);

            // ������ɫ
            SetPathObjectColor(obj, i, path.Count);

            // �������
            Debug.Log($"Դ��������: {sourceWorldPos}, Դ��Ļ����: {sourceScreenPos}, " +
                      $"��׼������: {normalizedScreenPos}, Ŀ����������: {targetWorldPos}");
        }
    }

    /// <summary>
    /// ����׼����Ļ����ת��ΪĿ�����������������
    /// </summary>
    private Vector3 ConvertNormalizedScreenToWorld(Vector2 normalizedScreenPos, Camera targetCamera)
    {
        // ת��Ϊ��Ļ��������
        float screenX = normalizedScreenPos.x * Screen.width;
        float screenY = normalizedScreenPos.y * Screen.height;

        // ������Ļ���꣨Zֵ����������������ã�
        Vector3 screenPos = new Vector3(screenX, screenY, 0);

        if (targetCamera.orthographic)
        {
            // �����������Z����Ϊ�����������С��һ��
            screenPos.z = targetCamera.orthographicSize;
        }
        else
        {
            // ͸���������Z����ΪĬ�����
            screenPos.z = defaultZDepth;
        }

        // ת��Ϊ��������
        return targetCamera.ScreenToWorldPoint(screenPos);
    }

    private void SetPathObjectColor(GameObject obj, int index, int totalCount)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer == null) return;

        if (index == 0) // ���
        {
            renderer.material.color = Color.green;
        }
        else if (index == totalCount - 1) // �յ�
        {
            renderer.material.color = Color.red;
        }
        else // �м��
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
using Fangtang;
using Sirenix.OdinInspector;
using UnityEngine;

public class InGameCameraSizeFit : ElementBehavior<InGameLineBee>
{
    public void FitSize()
    {
        FitCameraSize();
    }

    [Button]
    private void FitCameraSize()
    {
        //Use x
        //计算所有 Tile x最大值，用来计算出对应比例下y值，y值即为摄像机的 size
        Camera camera = Context.GameCamera;
        float maxX = GetAllPointMaxX() + PointConst.PointSize;
        float maxY = (float)Screen.height / Screen.width * maxX;
        //        Debug.Log($"Use x get camera size {maxY}");

        //Use y
        //计算所有 Tile y最大值
        float maxY_ = GetTileAllSubLevelMaxY() + PointConst.PointSize;
        //        Debug.Log($"Use Y get camera size {maxY_}  offset:{offset}");

        float targetY = Mathf.Max(maxY, maxY_);
        camera.orthographicSize = Mathf.Clamp(targetY, _minCameraSize, _maxCameraSize);
    }

    //根据 道具栏位置调整 slot 位置
    //    [Button]
    //    private void FitSlotPosition()
    //    {
    //        Camera camera = GameStage.Instance.Camera;
    //        Vector3 powerupScreenPoint = Context.GetView<InGamePlayingUI>().PowerupBarScreenPoint;
    //        powerupScreenPoint += Vector3.forward * 10f;
    //        Vector3 worldPosition = camera.ScreenToWorldPoint(powerupScreenPoint) + Vector3.up * _slotYOffset;
    //        GameStage.Instance.SlotProvider.transform.position = worldPosition;
    //    }

    //取所有子关卡 相对坐标中 Y 最大的值
    private float GetTileAllSubLevelMaxY()
    {
        float maxY = float.MinValue;
        Transform parent = GameStage.Instance.PointParent;

        for (int j = 0; j < parent.childCount; j++)
        {
            Transform child = parent.GetChild(j);
            float localChildY = GameStage.Instance.transform.position.y - child.position.y;

            if (Mathf.Abs(localChildY) > maxY)
            {
                maxY = child.position.y;
            }
        }

        return maxY;
    }

    //取相对坐标中 x  最大的值
    private float GetAllPointMaxX()
    {
        float maxX = float.MinValue;
        Transform parent = GameStage.Instance.PointParent;
        Vector3 center = GameStage.Instance.GetCenterPos(parent);

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);

            float localChildX = child.position.x - center.x;

            if (Mathf.Abs(localChildX) > maxX)
            {
                maxX = Mathf.Abs(localChildX);
            }
        }

        return maxX;
    }

    [Header("Camera")][SerializeField] private float _minCameraSize;

    [SerializeField] private float _maxCameraSize;
}
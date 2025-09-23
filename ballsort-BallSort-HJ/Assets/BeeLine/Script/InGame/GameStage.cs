using _02.Scripts.LevelEdit;
using ProjectSpace.BubbleMatch.Scripts.Util;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class GameStage : MonoSingleton<GameStage>
{
    [SerializeField] public Transform PointParent;
    [SerializeField] public Image bg;

    //根据生成的棋子调整位置，让所有棋子位置保持在中心
    [Button(ButtonSizes.Medium)]
    private void OnEnable()
    {
        SpriteManager.Instance.SetLineBeeBg(PlayerPrefs.GetInt("ClickThemeSkin"));
    }

    public void FitTileParentToCenter()
    {
        FitTileParent(PointParent);
    }

    public Vector3 GetCenterPos(Transform tileParent)
    {
        if (tileParent.childCount == 0)
            return Vector3.zero;

        float left, right, top, down;
        left = tileParent.GetChild(0).position.x;
        right = tileParent.GetChild(0).position.x;
        top = tileParent.GetChild(0).position.y;
        down = tileParent.GetChild(0).position.y;
        for (int i = 1; i < tileParent.childCount; i++)
        {
            Transform current = tileParent.GetChild(i);
            if (current.position.x < left)
            {
                left = current.position.x;
            }

            if (current.position.x > right)
            {
                right = current.position.x;
            }

            if (current.position.y < down)
            {
                down = current.position.y;
            }

            if (current.position.y > top)
            {
                top = current.position.y;
            }
        }

        return new Vector3((left + right) / 2f, (top + down) / 2f);
    }

    private void FitTileParent(Transform parent)
    {
        if (parent.childCount == 0)
            return;

        Vector3 center = GetCenterPos(parent);

        Vector3 moveOffset = parent.position - center;
        parent.position += moveOffset;
    }
}
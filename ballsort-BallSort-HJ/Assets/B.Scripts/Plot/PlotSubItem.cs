using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlotSubItem : MonoBehaviour
{
    public Vector2 oldPos;
    public Vector2 newPos;
    public float Delay = 0.5f;

    [SerializeField] private RectTransform rectT;
    [SerializeField] private CanvasGroup canvasGroup;

    public void Rest()
    {
        rectT.anchoredPosition = oldPos;
        canvasGroup.alpha = 0;
    }

    public void Play()
    {
        var seq = DOTween.Sequence();
        seq.Append(rectT.DOAnchorPos(newPos, Delay));
        seq.Join(canvasGroup.DOFade(1, Delay));
    }

    public void TestPlay()
    {
        rectT.anchoredPosition = newPos;
        canvasGroup.alpha = 1;
    }


    [Button]
    public void SetOldPos()
    {
        oldPos = rectT.anchoredPosition;
    }
    [Button]
    public void SetNewPos()
    {
        newPos = rectT.anchoredPosition;
    }
}
using UnityEngine;

public class InGameTutorialUI : ElementUI<InGameLineBee>
{
    [SerializeField] public RectTransform HandParent;

    public void SetHandRectPosY(float y)
    {
        _handRect.SetAnchoredPositionY(y + _offset);
    }

    [SerializeField] private RectTransform _handRect;
    [SerializeField] private float _offset = -50f;
}
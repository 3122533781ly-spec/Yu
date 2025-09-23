using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class InGameFailedUI : ElementUI<InGameLineBee>
{
    private void OnEnable()
    {
        _btnRestart.onClick.AddListener(ClickRestart);
        _groupContent.alpha = 0f;
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.15f);
        seq.Append(_groupContent.DOFade(1f, 0.3f));
    }

    private void OnDisable()
    {
        _btnRestart.onClick.RemoveListener(ClickRestart);
    }

    private void ClickRestart()
    {
        LineBee.Instance.EnterGame();
    }

    [SerializeField] private CanvasGroup _groupContent;
    [SerializeField] private Button _btnRestart;
}
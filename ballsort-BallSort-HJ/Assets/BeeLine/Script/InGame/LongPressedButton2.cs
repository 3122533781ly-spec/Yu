using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 长按判定按钮（带平滑恢复动画）
/// </summary>
public class LongPressedButton2 : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum EClickAnim
    {
        Scale,
        MoveY
    }

    [SerializeField] private bool interactable = true;
    [SerializeField][Range(0.5f, 3f)] private float longSeconds = 1.5f; // 长按触发时间
    [SerializeField] private Sprite StateOneSprite; // 状态1图标（如未长按）
    [SerializeField] private Sprite StateTwoSprite; // 状态2图标（如长按中）
    [SerializeField] private Sprite normalSprite;    // 正常状态图标
    [SerializeField] private Sprite disabledSprite;  // 禁用状态图标
    [SerializeField] private float changeValue = 0.8f; // 动画变化值（缩放/位移）
    [SerializeField] private float animationDuration = 0.2f; // 动画恢复时长
    [SerializeField] private EClickAnim clickAnim;

    private List<UnityAction> onLongClick = new List<UnityAction>();
    private bool isPressing = false; // 按下状态标记
    private bool isLongClickTriggered = false; // 长按触发标记
    private int currentState = 0; // 0: 正常状态, 1: 按下状态
    private RectTransform rect;
    private Image buttonImage; // 按钮主体Image

    public UnityEvent onClick;            // 普通点击事件
    public UnityEvent onLongClickStart;    // 长按开始事件
    public UnityEvent onLongClickComplete; // 长按完成事件

    public bool Interactable
    {
        set
        {
            interactable = value;
            if (buttonImage != null)
            {
                buttonImage.sprite = value ? normalSprite : disabledSprite;
                buttonImage.color = value ? Color.white : Color.gray;
            }
            ResetProgress(); // 禁用时重置进度条
        }
    }

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        ResetProgress(); // 初始化进度条
    }

    public void RegisterLongClick(UnityAction callback)
    {
        if (callback == null) return;
        onLongClick.Add(callback);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!interactable) return;

        isPressing = true;
        isLongClickTriggered = false;
        currentState = 1; // 标记为按下状态

        // 播放按下动画
        PlayPressAnim();
        // 重置并开始进度条
        ResetProgress();
        StartCoroutine(CountLongPressTime());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!interactable || !isPressing) return;

        isPressing = false;
        StopAllCoroutines(); // 停止长按计时

        if (isLongClickTriggered)
        {
            // 触发长按完成事件
            onLongClickComplete.Invoke();
            onLongClick.ForEach(c => c?.Invoke());
        }
        else
        {
            // 触发普通点击事件
            onClick.Invoke();
        }

        // 播放松开动画
        PlayReleaseAnim();
    }

    private IEnumerator CountLongPressTime() // 非泛型 IEnumerator
    {
        float elapsedTime = 0f;
        while (isPressing && elapsedTime < longSeconds)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= longSeconds && !isLongClickTriggered)
            {
                isLongClickTriggered = true;
                onLongClickStart.Invoke();
                UpdateStateSprite();
            }

            yield return null; // 非泛型 IEnumerator 允许 yield return null
        }
    }

    private void PlayPressAnim()
    {
        if (buttonImage != null && normalSprite != null)
        {
            buttonImage.sprite = normalSprite; // 按下时显示正常图标（可选）
        }

        switch (clickAnim)
        {
            case EClickAnim.Scale:
                rect.DOScale(changeValue, animationDuration).SetUpdate(true);
                break;

            case EClickAnim.MoveY:
                rect.DOLocalMoveY(changeValue, animationDuration).SetUpdate(true);
                break;
        }
    }

    private void PlayReleaseAnim()
    {
        switch (clickAnim)
        {
            case EClickAnim.Scale:
                rect.DOScale(1f, animationDuration).SetUpdate(true);
                break;

            case EClickAnim.MoveY:
                rect.DOLocalMoveY(0f, animationDuration).SetUpdate(true);
                break;
        }
        currentState = 0; // 恢复正常状态
        UpdateStateSprite(); // 切换状态图标
    }

    private void UpdateStateSprite()
    {
    }

    private void ResetProgress()
    {
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        rect.DOKill(); // 清理动画
        ResetProgress();
    }
}
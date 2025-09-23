using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// �����ж���ť����ƽ���ָ�������
/// </summary>
public class LongPressedButton2 : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum EClickAnim
    {
        Scale,
        MoveY
    }

    [SerializeField] private bool interactable = true;
    [SerializeField][Range(0.5f, 3f)] private float longSeconds = 1.5f; // ��������ʱ��
    [SerializeField] private Sprite StateOneSprite; // ״̬1ͼ�꣨��δ������
    [SerializeField] private Sprite StateTwoSprite; // ״̬2ͼ�꣨�糤���У�
    [SerializeField] private Sprite normalSprite;    // ����״̬ͼ��
    [SerializeField] private Sprite disabledSprite;  // ����״̬ͼ��
    [SerializeField] private float changeValue = 0.8f; // �����仯ֵ������/λ�ƣ�
    [SerializeField] private float animationDuration = 0.2f; // �����ָ�ʱ��
    [SerializeField] private EClickAnim clickAnim;

    private List<UnityAction> onLongClick = new List<UnityAction>();
    private bool isPressing = false; // ����״̬���
    private bool isLongClickTriggered = false; // �����������
    private int currentState = 0; // 0: ����״̬, 1: ����״̬
    private RectTransform rect;
    private Image buttonImage; // ��ť����Image

    public UnityEvent onClick;            // ��ͨ����¼�
    public UnityEvent onLongClickStart;    // ������ʼ�¼�
    public UnityEvent onLongClickComplete; // ��������¼�

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
            ResetProgress(); // ����ʱ���ý�����
        }
    }

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        ResetProgress(); // ��ʼ��������
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
        currentState = 1; // ���Ϊ����״̬

        // ���Ű��¶���
        PlayPressAnim();
        // ���ò���ʼ������
        ResetProgress();
        StartCoroutine(CountLongPressTime());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!interactable || !isPressing) return;

        isPressing = false;
        StopAllCoroutines(); // ֹͣ������ʱ

        if (isLongClickTriggered)
        {
            // ������������¼�
            onLongClickComplete.Invoke();
            onLongClick.ForEach(c => c?.Invoke());
        }
        else
        {
            // ������ͨ����¼�
            onClick.Invoke();
        }

        // �����ɿ�����
        PlayReleaseAnim();
    }

    private IEnumerator CountLongPressTime() // �Ƿ��� IEnumerator
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

            yield return null; // �Ƿ��� IEnumerator ���� yield return null
        }
    }

    private void PlayPressAnim()
    {
        if (buttonImage != null && normalSprite != null)
        {
            buttonImage.sprite = normalSprite; // ����ʱ��ʾ����ͼ�꣨��ѡ��
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
        currentState = 0; // �ָ�����״̬
        UpdateStateSprite(); // �л�״̬ͼ��
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
        rect.DOKill(); // ������
        ResetProgress();
    }
}
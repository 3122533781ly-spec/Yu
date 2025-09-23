using DG.Tweening;
using UnityEngine;

public class LoopHighlightEffect : MonoBehaviour
{
    [Tooltip("Ŀ�꾫����Ⱦ��")]
    [SerializeField] private SpriteRenderer targetSprite;

    [Tooltip("��˸���ڣ��룩")]
    [SerializeField] private float cycleDuration = 1.0f;

    [Tooltip("�߹���ɫ")]
    [SerializeField] private Color highlightColor = Color.white;

    [Tooltip("�Ƿ�����ʱ�Զ���ʼ��˸")]
    [SerializeField] private bool startOnEnable = true;

    [Tooltip("ԭʼ��ɫ�Ƿ������")]
    [SerializeField] private bool blendWithOriginalColor = true;

    private Color originalColor;
    private bool isFlickering = false;
    private bool LockLoop = false;

    private void OnEnable()
    {
        LockLoop = false;
        if (targetSprite == null)
            targetSprite = GetComponent<SpriteRenderer>();

        if (targetSprite == null)
        {
            Debug.LogError("SpriteRenderer not found!", gameObject);
            return;
        }

        originalColor = targetSprite.color;

        if (startOnEnable)
            StartFlicker();
    }

    private void OnDisable()
    {
        StopFlicker();
    }

    public void StartFlicker()
    {
        isFlickering = true;
    }

    public void StopFlicker()
    {
        isFlickering = false;
        targetSprite.color = originalColor;
    }

    private void Update()
    {
        if (!isFlickering)
            return;

        // ���㵱ǰ���ڵĽ��ȣ�0-1��
        float normalizedTime = (Time.time % cycleDuration) / cycleDuration;

        // ʹ�����Һ�������ƽ���Ľ���Ч����0-1-0ѭ����
        float fadeFactor = Mathf.Sin(normalizedTime * Mathf.PI);
        //if (normalizedTime < 0.2)
        //{
        //    LockLoop = true;
        //}
        //if (!LockLoop)
        //    return;
        // Ӧ����ɫ����
        Color finalColor = blendWithOriginalColor ?
            Color.Lerp(originalColor, highlightColor, fadeFactor) :
            SetAlpha(highlightColor, fadeFactor);

        targetSprite.color = finalColor;
    }

    private Color SetAlpha(Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }
}
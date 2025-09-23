using DG.Tweening;
using UnityEngine;

public class LoopHighlightEffect : MonoBehaviour
{
    [Tooltip("目标精灵渲染器")]
    [SerializeField] private SpriteRenderer targetSprite;

    [Tooltip("闪烁周期（秒）")]
    [SerializeField] private float cycleDuration = 1.0f;

    [Tooltip("高光颜色")]
    [SerializeField] private Color highlightColor = Color.white;

    [Tooltip("是否启用时自动开始闪烁")]
    [SerializeField] private bool startOnEnable = true;

    [Tooltip("原始颜色是否参与混合")]
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

        // 计算当前周期的进度（0-1）
        float normalizedTime = (Time.time % cycleDuration) / cycleDuration;

        // 使用正弦函数创建平滑的渐变效果（0-1-0循环）
        float fadeFactor = Mathf.Sin(normalizedTime * Mathf.PI);
        //if (normalizedTime < 0.2)
        //{
        //    LockLoop = true;
        //}
        //if (!LockLoop)
        //    return;
        // 应用颜色渐变
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
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour,IProgress
{
    public float CurrentPercentage
    {
        get
        {
            return _rectBar.fillAmount;
        }
    }

    public void Reset(float percentage)
    {
        UpdateProgress(percentage);
    }

    public void UpdateProgress(float percentage)
    {
        _rectBar.fillAmount = percentage;

        var posX = _rectBar.rectTransform.sizeDelta.x * percentage;
        // _pig.rectTransform.anchoredPosition = new Vector2(posX, 39);
    }


    [SerializeField] private Image _rectBar;
    // [SerializeField] private Image _pig;
}
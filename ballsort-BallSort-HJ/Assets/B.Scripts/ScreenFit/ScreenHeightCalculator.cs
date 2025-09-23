using UnityEngine;
using System.Collections;

public class ScreenHeightCalculator : MonoBehaviour
{
    [SerializeField] private RectTransform targetRect; // Ŀ��UIԪ��
    [SerializeField] private RectTransform targetRect2; // Ŀ��UIԪ��

    private void OnEnable()
    {
        // ѡ��һ�ַ�ʽ��
        // ��ʽ1������ִ�У����ӳ���־��
        ApplyScaledTopPadding();

        // ��ʽ2����ȫ�ӳ�ִ��
        // StartCoroutine(ApplyScaledTopPaddingWithDelay());
    }

    public float GetScaledScreenHeight()
    {
        StartCoroutine(ApplyScaledTopPaddingWithDelay()); // ��ȷ����Э��

        if (targetRect2 != null && targetRect2.sizeDelta.y > 2000)
            return 73f;
        else
            return 5f;
    }

    public void ApplyScaledTopPadding()
    {
        if (targetRect == null)
        {
            Debug.LogError("δָ��Ŀ��RectTransform");
            return;
        }

        float scaledPadding = GetScaledScreenHeight();
    }

    public IEnumerator ApplyScaledTopPaddingWithDelay()
    {
        if (targetRect == null)
        {
            Debug.LogError("δָ��Ŀ��RectTransform");
            yield break;
        }

        // �ӳ�0.2��
        yield return new WaitForSeconds(0f);

        // �ӳٺ�ִ�м���
        float scaledPadding = targetRect2 != null && targetRect2.sizeDelta.y > 2000 ? 73f : 5f;
        Debug.Log("��С" + scaledPadding + " " + (targetRect2 != null ? targetRect2.sizeDelta.y : 0));

        // Ӧ�ñ߾�
        targetRect.offsetMax = new Vector2(targetRect.offsetMax.x, -scaledPadding);
    }
}
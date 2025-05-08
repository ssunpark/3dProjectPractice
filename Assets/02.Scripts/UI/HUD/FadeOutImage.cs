using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutImage : MonoBehaviour
{
    public Image targetImage; // �帴�ϰ� ���� ��� �̹���
    public float duration = 1.0f; // ������� �� �ɸ��� �ð�

    public void StartFade()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        Color color = targetImage.color;
        float startAlpha = color.a;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            float newAlpha = Mathf.Lerp(startAlpha, 0f, t);
            targetImage.color = new Color(color.r, color.g, color.b, newAlpha);
            yield return null;
        }

        // Ȯ���ϰ� 0���� ����
        targetImage.color = new Color(color.r, color.g, color.b, 0f);
    }
}

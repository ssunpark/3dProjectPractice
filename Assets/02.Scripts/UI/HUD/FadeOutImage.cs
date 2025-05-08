using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutImage : MonoBehaviour
{
    public Image targetImage; // 흐릿하게 만들 대상 이미지
    public float duration = 1.0f; // 흐려지는 데 걸리는 시간

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

        // 확실하게 0으로 고정
        targetImage.color = new Color(color.r, color.g, color.b, 0f);
    }
}

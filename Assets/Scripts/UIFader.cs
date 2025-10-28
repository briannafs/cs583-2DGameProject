using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIFader : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float duration = 0.5f;

    IEnumerator Start()
    {
        if (!fadeImage) yield break;
        var c = fadeImage.color; c.a = 1f; fadeImage.color = c;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / duration);
            fadeImage.color = c;
            yield return null;
        }
        fadeImage.gameObject.SetActive(false);
    }
}

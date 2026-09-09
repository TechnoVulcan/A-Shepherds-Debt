using System.Collections;
using UnityEngine;

public class FogFade : MonoBehaviour
{
    [SerializeField] private SpriteRenderer fogRenderer;
    [SerializeField] private float fadeDuration = 2f;

    [Header("Lighting")]
    [SerializeField] private MazeLightingController mazeLighting;

    [Header("Random Maze Audio")]
    [SerializeField] private MazeRandomAudio mazeRandomAudio;

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        SetAlpha(0f);
    }

    public void FadeIn()
    {
        Debug.Log("===== FOG FADE IN CALLED =====");

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        if (mazeLighting != null)
        {
            mazeLighting.EnableMazeLighting();
        }

        if (mazeRandomAudio != null)
        {
            mazeRandomAudio.StartRandomSounds();
        }

        fadeCoroutine = StartCoroutine(Fade(0f, 1f));
    }

    public void FadeOut()
    {
        Debug.Log("===== FOG FADE OUT CALLED =====");

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        if (mazeLighting != null)
        {
            mazeLighting.DisableMazeLighting();
        }

        if (mazeRandomAudio != null)
        {
            mazeRandomAudio.StopRandomSounds();
        }

        fadeCoroutine = StartCoroutine(Fade(1f, 0f));
    }

    public IEnumerator FadeOutAndWait()
    {
        Debug.Log("===== FOG FADE OUT AND WAIT CALLED =====");

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        if (mazeLighting != null)
        {
            mazeLighting.DisableMazeLighting();
        }

        if (mazeRandomAudio != null)
        {
            mazeRandomAudio.StopRandomSounds();
        }

        yield return StartCoroutine(Fade(1f, 0f));

        Debug.Log("===== FOG FADE OUT COMPLETE =====");
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(
                elapsed / fadeDuration
            );

            t = Mathf.SmoothStep(
                0f,
                1f,
                t
            );

            float alpha = Mathf.Lerp(
                startAlpha,
                endAlpha,
                t
            );

            SetAlpha(alpha);

            yield return null;
        }

        SetAlpha(endAlpha);

        fadeCoroutine = null;
    }

    private void SetAlpha(float alpha)
    {
        if (fogRenderer == null)
            return;

        Color color = fogRenderer.color;
        color.a = alpha;
        fogRenderer.color = color;
    }
}
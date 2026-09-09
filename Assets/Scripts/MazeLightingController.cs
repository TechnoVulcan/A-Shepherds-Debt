using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MazeLightingController : MonoBehaviour
{
    [Header("Lights")]
    [SerializeField] private Light2D globalLight;
    [SerializeField] private Light2D playerLight;

    [Header("Settings")]
    [SerializeField] private float normalIntensity = 0.25f;
    [SerializeField] private float mazeIntensity = 0.05f;
    [SerializeField] private float fadeDuration = 2f;

    private Coroutine lightingCoroutine;

    private void Start()
    {
        globalLight.intensity = normalIntensity;
        playerLight.enabled = false;
        playerLight.intensity = 1f;
    }

    public void EnableMazeLighting()
    {
        if (lightingCoroutine != null)
            StopCoroutine(lightingCoroutine);

        playerLight.enabled = true;
        lightingCoroutine = StartCoroutine(ChangeLighting(
            normalIntensity,
            mazeIntensity,
            0f,
            1f
        ));
    }

    public void DisableMazeLighting()
    {
        if (lightingCoroutine != null)
            StopCoroutine(lightingCoroutine);

        lightingCoroutine = StartCoroutine(ChangeLighting(
            mazeIntensity,
            normalIntensity,
            1f,
            0f
        ));
    }

    private IEnumerator ChangeLighting(
        float globalStart,
        float globalTarget,
        float playerStart,
        float playerTarget)
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / fadeDuration);

            // Smooth transition
            t = Mathf.SmoothStep(0f, 1f, t);

            globalLight.intensity = Mathf.Lerp(
                globalStart,
                globalTarget,
                t
            );

            playerLight.intensity = Mathf.Lerp(
                playerStart,
                playerTarget,
                t
            );

            yield return null;
        }

        globalLight.intensity = globalTarget;
        playerLight.intensity = playerTarget;

        if (playerTarget == 0f)
            playerLight.enabled = false;

        lightingCoroutine = null;
    }
}
using System.Collections;
using UnityEngine;

public class EndingManager : MonoBehaviour
{
    public static EndingManager Instance;

    [SerializeField] private CanvasGroup blackScreen;
    [SerializeField] private GameObject blackScreenImage;
    [SerializeField] private RectTransform canvasRect;

    [SerializeField] private GameObject jumpScare;
    [SerializeField] private GameObject scream;
    [SerializeField] private GameObject townText;

    [SerializeField] private GameObject girlName;

    [SerializeField] private GameObject ominousText;

    [SerializeField] private GameObject credits;

    [SerializeField] private MonoBehaviour playerMovement;

    private Vector2 originalCanvasPos;
    private bool endingPlaying = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (canvasRect != null)
        {
            originalCanvasPos = canvasRect.anchoredPosition;
        }

        HideBlackScreen();

        if (jumpScare != null)
            jumpScare.SetActive(false);

        if (scream != null)
            scream.SetActive(false);

        if (townText != null)
            townText.SetActive(false);

        if (girlName != null)
            girlName.SetActive(false);

        if (ominousText != null)
            ominousText.SetActive(false);

        if (credits != null)
            credits.SetActive(false);
    }

    public void PlayEnding(int endingNumber)
    {
        Debug.Log("========================================");
        Debug.Log("ENDING MANAGER RECEIVED ENDING NUMBER = " + endingNumber);
        Debug.Log("========================================");

        if (endingPlaying)
        {
            Debug.LogWarning("An ending is already playing.");
            return;
        }

        endingPlaying = true;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopAllBackgroundAudio();
            AudioManager.Instance.StopStatueInteractionBackground();
        }

        StartCoroutine(EndingSequence(endingNumber));
    }

    private IEnumerator EndingSequence(int endingNumber)
    {
        switch (endingNumber)
        {
            case 1:
                Debug.Log(">>> PLAYING ENDING 1 <<<");

                yield return StartCoroutine(EndingOne());

                break;

            case 2:
                Debug.Log(">>> PLAYING ENDING 2 <<<");

                yield return StartCoroutine(EndingTwo());

                break;

            case 3:
                Debug.Log(">>> PLAYING ENDING 3 <<<");

                yield return StartCoroutine(EndingThree());

                break;

            default:
                Debug.LogError(
                    "INVALID ENDING NUMBER RECEIVED: " +
                    endingNumber
                );

                endingPlaying = false;

                yield break;
        }
    }

    private void ActivateEndingScreen()
    {
        if (blackScreen != null)
        {
            blackScreen.alpha = 1f;
            blackScreen.interactable = false;
            blackScreen.blocksRaycasts = false;
        }

        if (blackScreenImage != null)
        {
            blackScreenImage.SetActive(true);
            blackScreenImage.transform.SetAsFirstSibling();
        }
    }

    private void HideBlackScreen()
    {
        if (blackScreen != null)
        {
            blackScreen.alpha = 0f;
            blackScreen.interactable = false;
            blackScreen.blocksRaycasts = false;
        }

        if (blackScreenImage != null)
        {
            blackScreenImage.SetActive(false);
        }
    }

    private IEnumerator EndingOne()
    {
        FreezePlayer();

        if (girlName != null)
        {
            girlName.SetActive(true);
            girlName.transform.SetAsLastSibling();
        }

        yield return new WaitForSeconds(2.5f);

        if (girlName != null)
        {
            girlName.SetActive(false);
        }

        ShowCredits();

        yield return new WaitForSeconds(8f);
    }

    private IEnumerator EndingTwo()
    {
        FreezePlayer();

        if (ominousText != null)
        {
            ominousText.SetActive(true);
            ominousText.transform.SetAsLastSibling();
        }

        yield return new WaitForSeconds(8f);

        if (ominousText != null)
        {
            ominousText.SetActive(false);
        }

        ShowCredits();

        yield return new WaitForSeconds(8f);
    }

    private IEnumerator EndingThree()
    {
        ActivateEndingScreen();

        yield return new WaitForSeconds(0.3f);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayStoneGrinding();
        }

        yield return new WaitForSeconds(1.2f);

        if (blackScreen != null)
        {
            blackScreen.alpha = 1f;
        }

        yield return new WaitForSeconds(0.2f);

        FreezePlayer();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopStoneGrinding();
            AudioManager.Instance.PlayJumpscareMusic();
        }

        if (jumpScare != null)
        {
            jumpScare.SetActive(true);
            jumpScare.transform.SetAsLastSibling();
        }

        yield return new WaitForSeconds(0.7f);

        if (jumpScare != null)
        {
            jumpScare.SetActive(false);
        }

        if (scream != null)
        {
            scream.SetActive(true);
            scream.transform.SetAsLastSibling();
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayScream();
        }

        yield return StartCoroutine(
            ShakeCanvas(
                0.5f,
                15f
            )
        );

        if (scream != null)
        {
            scream.SetActive(false);
        }

        if (townText != null)
        {
            townText.SetActive(true);
            townText.transform.SetAsLastSibling();
        }

        yield return new WaitForSeconds(4f);

        if (townText != null)
        {
            townText.SetActive(false);
        }

        ShowCredits();

        yield return new WaitForSeconds(8f);
    }

    private void FreezePlayer()
    {
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
    }

    private void ShowCredits()
    {
        if (credits == null)
            return;

        ActivateEndingScreen();

        credits.SetActive(true);
        credits.transform.SetAsLastSibling();
    }

    private IEnumerator ShakeCanvas(
        float duration,
        float magnitude
    )
    {
        if (canvasRect == null)
            yield break;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            canvasRect.anchoredPosition =
                originalCanvasPos +
                Random.insideUnitCircle *
                magnitude;

            elapsed += Time.deltaTime;

            yield return null;
        }

        canvasRect.anchoredPosition = originalCanvasPos;
    }
}
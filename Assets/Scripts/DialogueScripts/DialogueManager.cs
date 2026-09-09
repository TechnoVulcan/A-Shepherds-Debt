using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Playables;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [SerializeField] private GameObject Girl;
    [SerializeField] private GameObject noExitTrigger;
    [SerializeField] private GameObject fogController;
    [SerializeField] private FogFade fogFade;
    [SerializeField] private MazeLightingController mazeLighting;
    [SerializeField] private MazeRandomAudio mazeRandomAudio;

    [SerializeField] private GameObject cutscenePlayer;
    [SerializeField] private GameObject realPlayer;

    [SerializeField] private EndingManager endingManager;

    public CanvasGroup canvasGroup;
    public Image portrait;
    public TMP_Text actorName;
    public TMP_Text dialogueText;

    [SerializeField] private Image statueBackground;

    [SerializeField] private float fadeDuration = 0.25f;

    [SerializeField] private ScreenFade screenFade;
    [SerializeField] private Transform player;
    [SerializeField] private Transform wingedStatueSpawn;

    [SerializeField] private PlayableDirector timelineDirector;

    [SerializeField] private GameObject leftBarrier;
    [SerializeField] private GameObject rightBarrier;

    public bool isDialogueActive;
    public bool justClosedDialogue;

    private DialogueSO currentDialogue;
    private int dialogueIndex;

    private bool hasSwappedPlayers = false;
    private bool waitingForChoice = false;

    private Coroutine autoAdvanceCoroutine;
    private Coroutine fadeCoroutine;
    private Coroutine timelineCoroutine;
    private Coroutine teleportCoroutine;

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

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        if (statueBackground != null)
        {
            statueBackground.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isDialogueActive)
            return;

        if (waitingForChoice)
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                ChooseYes();
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                ChooseNo();
            }

            return;
        }

        if (Input.GetButtonDown("Interact"))
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayInteract();
            }

            AdvanceDialogue();
        }
    }

    public void StartDialogue(DialogueSO dialogueSO)
    {
        if (dialogueSO == null)
        {
            Debug.LogError(
                "StartDialogue called with NULL DialogueSO."
            );

            return;
        }

        Debug.Log(
            "DIALOGUE MANAGER STARTING: " +
            dialogueSO.name +
            " | startEnding = " +
            dialogueSO.startEnding +
            " | endingNumber = " +
            dialogueSO.endingNumber
        );

        if (timelineCoroutine != null)
        {
            StopCoroutine(timelineCoroutine);
            timelineCoroutine = null;
        }

        if (autoAdvanceCoroutine != null)
        {
            StopCoroutine(autoAdvanceCoroutine);
            autoAdvanceCoroutine = null;
        }

        if (!hasSwappedPlayers &&
            cutscenePlayer != null &&
            realPlayer != null)
        {
            realPlayer.transform.position =
                cutscenePlayer.transform.position;

            realPlayer.transform.rotation =
                cutscenePlayer.transform.rotation;

            player = realPlayer.transform;

            realPlayer.SetActive(true);
            cutscenePlayer.SetActive(false);

            hasSwappedPlayers = true;
        }

        currentDialogue = dialogueSO;
        dialogueIndex = 0;
        waitingForChoice = false;
        isDialogueActive = true;

        if (currentDialogue.enableChoice)
        {
            if (leftBarrier != null)
                leftBarrier.SetActive(false);

            if (rightBarrier != null)
                rightBarrier.SetActive(false);
        }

        if (currentDialogue.enableStatueBackground)
        {
            if (statueBackground != null)
            {
                statueBackground.gameObject.SetActive(true);
                UpdateStatueBackground();
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayMazeAmbience();
            }
        }
        else
        {
            if (statueBackground != null)
            {
                statueBackground.gameObject.SetActive(false);
            }
        }

        if (currentDialogue.enableStatueInteractionAudio)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayStatueInteractionBackground();
            }
        }

        if (currentDialogue.playChurchBells)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayChurchBells();
            }
        }

        if (currentDialogue.enableFog)
        {
            if (fogController != null)
                fogController.SetActive(true);

            if (fogFade != null)
                fogFade.FadeIn();

            if (mazeLighting != null)
                mazeLighting.EnableMazeLighting();

            if (mazeRandomAudio != null)
                mazeRandomAudio.StartRandomSounds();
        }

        ShowDialogue();

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine =
            StartCoroutine(
                FadeCanvas(1f)
            );
    }

    public void AdvanceDialogue()
    {
        if (!isDialogueActive)
            return;

        if (waitingForChoice)
            return;

        if (currentDialogue == null)
        {
            Debug.LogError(
                "AdvanceDialogue called but currentDialogue is NULL."
            );

            return;
        }

        if (currentDialogue.lines == null ||
            currentDialogue.lines.Length == 0)
        {
            EndDialogue();
            return;
        }

        if (dialogueIndex < currentDialogue.lines.Length)
        {
            ShowDialogue();
        }
        else
        {
            EndDialogue();
        }
    }

    private void ShowDialogue()
    {
        if (currentDialogue == null)
            return;

        if (currentDialogue.lines == null ||
            currentDialogue.lines.Length == 0)
        {
            EndDialogue();
            return;
        }

        if (dialogueIndex >= currentDialogue.lines.Length)
        {
            EndDialogue();
            return;
        }

        UpdateStatueBackground();

        DialogueLine line =
            currentDialogue.lines[dialogueIndex];

        if (line == null)
        {
            dialogueIndex++;
            return;
        }

        if (line.speaker != null)
        {
            if (portrait != null)
                portrait.sprite = line.speaker.portrait;

            if (actorName != null)
                actorName.text = line.speaker.actorName;
        }
        else
        {
            if (portrait != null)
                portrait.sprite = null;

            if (actorName != null)
                actorName.text = "";
        }

        if (dialogueText != null)
        {
            dialogueText.text = line.text;
        }

        if (currentDialogue.makeGirlDisappear &&
            dialogueIndex == currentDialogue.disappearAtLine)
        {
            if (Girl != null)
            {
                Girl.SetActive(false);
            }
        }

        if (currentDialogue.enableChoice &&
            dialogueIndex == currentDialogue.choiceLineIndex)
        {
            waitingForChoice = true;

            if (dialogueText != null)
            {
                dialogueText.text +=
                    "\n\n[Y] Yes     [N] No";
            }
        }

        if (canvasGroup != null)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        dialogueIndex++;

        if (currentDialogue.autoAdvance &&
            !waitingForChoice)
        {
            if (autoAdvanceCoroutine != null)
            {
                StopCoroutine(autoAdvanceCoroutine);
            }

            autoAdvanceCoroutine =
                StartCoroutine(
                    AutoAdvanceDialogue()
                );
        }
    }

    private IEnumerator AutoAdvanceDialogue()
    {
        float delay =
            Mathf.Max(
                0.1f,
                currentDialogue.autoAdvanceDelay
            );

        yield return new WaitForSeconds(delay);

        autoAdvanceCoroutine = null;

        if (!isDialogueActive)
            yield break;

        if (waitingForChoice)
            yield break;

        AdvanceDialogue();
    }

    private void UpdateStatueBackground()
    {
        if (currentDialogue == null)
            return;

        if (!currentDialogue.enableStatueBackground)
            return;

        if (statueBackground == null)
            return;

        if (currentDialogue.statueBackgrounds == null ||
            currentDialogue.statueBackgrounds.Length == 0)
            return;

        StatueBackground selectedBackground = null;

        for (int i = 0;
             i < currentDialogue.statueBackgrounds.Length;
             i++)
        {
            StatueBackground background =
                currentDialogue.statueBackgrounds[i];

            if (background == null)
                continue;

            if (dialogueIndex >= background.changeAtLine)
            {
                selectedBackground = background;
            }
        }

        if (selectedBackground != null)
        {
            statueBackground.sprite =
                selectedBackground.backgroundImage;

            statueBackground.gameObject.SetActive(true);
        }
    }

    private void ChooseYes()
    {
        waitingForChoice = false;

        if (autoAdvanceCoroutine != null)
        {
            StopCoroutine(autoAdvanceCoroutine);
            autoAdvanceCoroutine = null;
        }

        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.SetKarma(2);

            Debug.Log(
                "CHOICE YES | Karma set to " +
                GameProgress.Instance.karma
            );
        }

        if (rightBarrier != null)
        {
            rightBarrier.SetActive(true);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayInteract();
        }

        if (currentDialogue != null &&
            currentDialogue.yesDialogue != null)
        {
            currentDialogue =
                currentDialogue.yesDialogue;

            dialogueIndex = 0;

            Debug.Log(
                "YES DIALOGUE -> " +
                currentDialogue.name +
                " | endingNumber = " +
                currentDialogue.endingNumber
            );

            PrepareNewDialogueState();

            ShowDialogue();
        }
        else
        {
            Debug.LogError(
                "YES DIALOGUE IS NOT ASSIGNED."
            );

            EndDialogue();
        }
    }

    private void ChooseNo()
    {
        waitingForChoice = false;

        if (autoAdvanceCoroutine != null)
        {
            StopCoroutine(autoAdvanceCoroutine);
            autoAdvanceCoroutine = null;
        }

        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.SetKarma(0);

            Debug.Log(
                "CHOICE NO | Karma set to " +
                GameProgress.Instance.karma
            );
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayInteract();
        }

        if (currentDialogue != null &&
            currentDialogue.noDialogue != null)
        {
            currentDialogue =
                currentDialogue.noDialogue;

            dialogueIndex = 0;

            Debug.Log(
                "NO DIALOGUE -> " +
                currentDialogue.name +
                " | endingNumber = " +
                currentDialogue.endingNumber
            );

            PrepareNewDialogueState();

            ShowDialogue();
        }
        else
        {
            Debug.LogError(
                "NO DIALOGUE IS NOT ASSIGNED."
            );

            EndDialogue();
        }
    }

    private void PrepareNewDialogueState()
    {
        if (currentDialogue.enableStatueBackground)
        {
            if (statueBackground != null)
            {
                statueBackground.gameObject.SetActive(true);
                UpdateStatueBackground();
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayMazeAmbience();
            }
        }
        else
        {
            if (statueBackground != null)
            {
                statueBackground.gameObject.SetActive(false);
            }
        }

        if (currentDialogue.enableStatueInteractionAudio)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayStatueInteractionBackground();
            }
        }

        if (currentDialogue.playChurchBells)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayChurchBells();
            }
        }

        if (currentDialogue.enableFog)
        {
            if (mazeRandomAudio != null)
            {
                mazeRandomAudio.StartRandomSounds();
            }
        }
    }

    private void EndDialogue()
    {
        if (currentDialogue == null)
            return;

        DialogueSO finishedDialogue =
            currentDialogue;

        Debug.Log("========================================");
        Debug.Log("DIALOGUE FINISHED");
        Debug.Log(
            "DialogueSO = " +
            finishedDialogue.name
        );
        Debug.Log(
            "startEnding = " +
            finishedDialogue.startEnding
        );
        Debug.Log(
            "endingNumber = " +
            finishedDialogue.endingNumber
        );

        if (GameProgress.Instance != null)
        {
            Debug.Log(
                "Karma at dialogue end = " +
                GameProgress.Instance.karma
            );
        }

        Debug.Log("========================================");

        if (autoAdvanceCoroutine != null)
        {
            StopCoroutine(autoAdvanceCoroutine);
            autoAdvanceCoroutine = null;
        }

        if (finishedDialogue.enableStatueInteractionAudio)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance
                    .StopStatueInteractionBackground();
            }
        }

        if (finishedDialogue.unlockGirlBarrier)
        {
            if (GameProgress.Instance != null)
            {
                GameProgress.Instance.talkedToGirl = true;
            }
        }

        if (finishedDialogue.lockedExit)
        {
            if (GameProgress.Instance != null)
            {
                GameProgress.Instance.NoExit = true;
            }

            if (noExitTrigger != null)
            {
                noExitTrigger.SetActive(true);
            }
        }

        if (finishedDialogue.talkedToStatue)
        {
            if (GameProgress.Instance != null)
            {
                GameProgress.Instance.talkedToStatue = true;
            }
        }

        if (finishedDialogue.teleportToStatue)
        {
            if (teleportCoroutine != null)
            {
                StopCoroutine(teleportCoroutine);
            }

            teleportCoroutine =
                StartCoroutine(
                    TeleportBackToStatue()
                );
        }

        if (statueBackground != null)
        {
            statueBackground.gameObject.SetActive(false);
        }

        dialogueIndex = 0;
        waitingForChoice = false;
        isDialogueActive = false;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine =
            StartCoroutine(
                FadeOutDialogue()
            );

        StartCoroutine(
            DialogueCloseCooldown()
        );

        if (finishedDialogue.startEnding)
        {
            Debug.Log(
                "ENDING TRIGGERED FROM DIALOGUE"
            );

            Debug.Log(
                "SENDING ENDING NUMBER = " +
                finishedDialogue.endingNumber
            );

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance
                    .StopAllBackgroundAudio();

                AudioManager.Instance
                    .StopStatueInteractionBackground();
            }

            if (endingManager != null)
            {
                endingManager.PlayEnding(
                    finishedDialogue.endingNumber
                );
            }
            else
            {
                Debug.LogError(
                    "Ending Manager is not assigned in DialogueManager."
                );
            }

            return;
        }

        if (finishedDialogue.playTimelineAfterDialogue)
        {
            if (finishedDialogue.timelineToPlay == null)
            {
                Debug.LogError(
                    "Timeline is enabled but no Timeline asset is assigned in DialogueSO '" +
                    finishedDialogue.name +
                    "'."
                );
            }
            else if (timelineDirector == null)
            {
                Debug.LogError(
                    "Timeline is enabled but Timeline Director is not assigned in DialogueManager."
                );
            }
            else
            {
                if (timelineCoroutine != null)
                {
                    StopCoroutine(timelineCoroutine);
                }

                timelineCoroutine =
                    StartCoroutine(
                        PlayTimelineAfterDialogue(
                            finishedDialogue
                        )
                    );

                return;
            }
        }
    }

    private IEnumerator PlayTimelineAfterDialogue(
        DialogueSO dialogue
    )
    {
        yield return new WaitForSeconds(0.25f);

        timelineDirector.Stop();

        timelineDirector.playableAsset =
            dialogue.timelineToPlay;

        timelineDirector.time = 0;

        timelineDirector.Evaluate();

        timelineDirector.Play();

        while (timelineDirector.state ==
               PlayState.Playing)
        {
            yield return null;
        }

        timelineDirector.Stop();

        timelineCoroutine = null;

        if (dialogue.dialogueAfterTimeline != null)
        {
            StartDialogue(
                dialogue.dialogueAfterTimeline
            );
        }
        else
        {
            Debug.LogWarning(
                "Timeline finished but DialogueSO '" +
                dialogue.name +
                "' has no dialogueAfterTimeline assigned."
            );
        }
    }

    private IEnumerator FadeCanvas(float targetAlpha)
    {
        if (canvasGroup == null)
            yield break;

        float startAlpha =
            canvasGroup.alpha;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float t =
                elapsed / fadeDuration;

            canvasGroup.alpha =
                Mathf.Lerp(
                    startAlpha,
                    targetAlpha,
                    t
                );

            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        fadeCoroutine = null;
    }

    private IEnumerator FadeOutDialogue()
    {
        if (canvasGroup != null)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        yield return StartCoroutine(
            FadeCanvas(0f)
        );
    }

    private IEnumerator TeleportBackToStatue()
    {
        if (screenFade != null)
        {
            screenFade.FadeOut();
        }

        yield return new WaitForSeconds(2f);

        if (player != null &&
            wingedStatueSpawn != null)
        {
            player.position =
                wingedStatueSpawn.position;
        }

        yield return new WaitForSeconds(0.3f);

        if (screenFade != null)
        {
            screenFade.FadeIn();
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopChurchBells();
            AudioManager.Instance.PlayIntroAmbience();
            AudioManager.Instance.PlayMazeAmbience();
        }

        teleportCoroutine = null;
    }

    private IEnumerator DialogueCloseCooldown()
    {
        justClosedDialogue = true;

        yield return null;

        justClosedDialogue = false;
    }
}
using UnityEngine;
using System.Collections;

public class LocationVisitedTrigger : MonoBehaviour
{
    [SerializeField] private LocationSO locationVisited;
    [SerializeField] private DialogueSO dialogueToPlay;
    [SerializeField] private FogFade fogFade;
    [SerializeField] private bool isChurchScene;

    [Header("Statue Dialogue")]
    [SerializeField] private bool isStatueScene;

    [SerializeField] private DialogueSO ending1Dialogue;
    [SerializeField] private DialogueSO ending2Dialogue;
    [SerializeField] private DialogueSO ending3Dialogue;

    private bool triggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered)
            return;

        if (!collision.CompareTag("Player"))
            return;

        if (GameProgress.Instance == null)
        {
            Debug.LogError("GameProgress.Instance is NULL.");
            return;
        }

        if (dialogueToPlay != null &&
            dialogueToPlay.name == "NoExit" &&
            !GameProgress.Instance.NoExit)
        {
            return;
        }

        triggered = true;

        if (isChurchScene)
        {
            GameProgress.Instance.SetKarma(-1);

            Debug.Log(
                "CHURCH ENTERED | Karma changed to: " +
                GameProgress.Instance.karma
            );
        }

        if (LocationHistoryTracker.Instance != null)
        {
            LocationHistoryTracker.Instance.RecordLocation(
                locationVisited
            );
        }

        if (dialogueToPlay != null &&
            dialogueToPlay.enableFog)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayMazeAmbience();
            }

            MazeRandomAudio mazeAudio =
                FindFirstObjectByType<MazeRandomAudio>();

            if (mazeAudio != null)
            {
                mazeAudio.StartRandomSounds();
            }
        }

        DialogueSO dialogueToStart = dialogueToPlay;

        if (isStatueScene)
        {
            int karma = GameProgress.Instance.karma;

            Debug.Log("========================================");
            Debug.Log("STATUE TRIGGER REACHED");
            Debug.Log("CURRENT KARMA = " + karma);

            Debug.Log(
                "ENDING 1 SO = " +
                (ending1Dialogue != null
                    ? ending1Dialogue.name
                    : "NULL")
            );

            Debug.Log(
                "ENDING 2 SO = " +
                (ending2Dialogue != null
                    ? ending2Dialogue.name
                    : "NULL")
            );

            Debug.Log(
                "ENDING 3 SO = " +
                (ending3Dialogue != null
                    ? ending3Dialogue.name
                    : "NULL")
            );

            if (karma == 2)
            {
                dialogueToStart = ending1Dialogue;

                Debug.Log(
                    ">>> KARMA = 2 -> SELECTING ENDING 1 DIALOGUE <<<"
                );
            }
            else if (karma == 0)
            {
                dialogueToStart = ending2Dialogue;

                Debug.Log(
                    ">>> KARMA = 0 -> SELECTING ENDING 2 DIALOGUE <<<"
                );
            }
            else if (karma < 0)
            {
                dialogueToStart = ending3Dialogue;

                Debug.Log(
                    ">>> KARMA < 0 -> SELECTING ENDING 3 DIALOGUE <<<"
                );
            }
            else
            {
                Debug.LogError(
                    "UNEXPECTED KARMA VALUE: " + karma
                );

                dialogueToStart = ending3Dialogue;
            }

            if (dialogueToStart != null)
            {
                Debug.Log(
                    "SELECTED DIALOGUE SO = " +
                    dialogueToStart.name
                );

                Debug.Log(
                    "SELECTED DIALOGUE startEnding = " +
                    dialogueToStart.startEnding
                );

                Debug.Log(
                    "SELECTED DIALOGUE endingNumber = " +
                    dialogueToStart.endingNumber
                );
            }
            else
            {
                Debug.LogError(
                    "SELECTED ENDING DIALOGUE IS NULL."
                );
            }

            Debug.Log("========================================");
        }

        if (dialogueToStart == null)
        {
            Debug.LogWarning(
                "LocationVisitedTrigger has no DialogueSO assigned."
            );

            Destroy(gameObject);
            return;
        }

        if (fogFade != null)
        {
            StartCoroutine(
                StartDialogueAfterFog(
                    dialogueToStart
                )
            );

            return;
        }

        if (DialogueManager.Instance != null)
        {
            Debug.Log(
                "STARTING SELECTED DIALOGUE: " +
                dialogueToStart.name
            );

            DialogueManager.Instance.StartDialogue(
                dialogueToStart
            );
        }
        else
        {
            Debug.LogError(
                "DialogueManager.Instance is NULL."
            );
        }

        Destroy(gameObject);
    }

    private IEnumerator StartDialogueAfterFog(
        DialogueSO dialogue
    )
    {
        yield return StartCoroutine(
            fogFade.FadeOutAndWait()
        );

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMazeAmbience();
        }

        MazeRandomAudio mazeAudio =
            FindFirstObjectByType<MazeRandomAudio>();

        if (mazeAudio != null)
        {
            mazeAudio.StopRandomSounds();
        }

        if (isChurchScene &&
            AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayChurchBells();
        }

        Debug.Log(
            "===== FOG FINISHED - STARTING DIALOGUE ====="
        );

        Debug.Log(
            "DIALOGUE AFTER FOG = " +
            dialogue.name
        );

        Debug.Log(
            "startEnding = " +
            dialogue.startEnding
        );

        Debug.Log(
            "endingNumber = " +
            dialogue.endingNumber
        );

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(
                dialogue
            );
        }
        else
        {
            Debug.LogError(
                "DialogueManager.Instance is NULL."
            );
        }

        Destroy(gameObject);
    }
}
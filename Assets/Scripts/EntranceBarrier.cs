using UnityEngine;

public class EntranceBarrier : MonoBehaviour
{
    [SerializeField] private DialogueSO talkToGirlDialogue;
    [SerializeField] private GameObject entranceWall;

    private bool dialoguePlaying = false;

    private void Update()
    {
        // Beginning - entrance blocked
        if (!GameProgress.Instance.talkedToGirl)
        {
            entranceWall.SetActive(true);
            return;
        }

        // Talked to girl - entrance opens
        if (GameProgress.Instance.talkedToGirl &&
            !GameProgress.Instance.NoExit)
        {
            entranceWall.SetActive(false);
            return;
        }

        // Fog dialogue happened - entrance closes again
        if (GameProgress.Instance.NoExit)
        {
            entranceWall.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (!GameProgress.Instance.talkedToGirl && !dialoguePlaying)
        {
            dialoguePlaying = true;
            DialogueManager.Instance.StartDialogue(talkToGirlDialogue);

            // Prevent this trigger from firing again
            GetComponent<Collider2D>().enabled = false;
        }
    }
}
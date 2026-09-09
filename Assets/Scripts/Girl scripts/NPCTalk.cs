using UnityEngine;

public class NPCTalk : MonoBehaviour
{
    public Animator interactAnim;
    public DialogueSO dialogueSO;
    public GameObject interactIcon;

    private bool playerInRange;

    private void OnEnable()
    {
        if (interactAnim != null)
        {
            interactAnim.Play("open");
        }
    }

    private void OnDisable()
    {
        if (interactAnim != null)
        {
            interactAnim.Play("close");
        }
    }

    private void Update()
    {
        if (!playerInRange)
            return;

        if (DialogueManager.Instance.justClosedDialogue)
            return;

        // Girl has already been talked to
        if (GameProgress.Instance.talkedToGirl)
        {
            if (interactIcon != null && interactIcon.activeSelf)
            {
                interactIcon.SetActive(false);
            }

            return;
        }

        if (Input.GetButtonDown("Interact"))
        {
            if (!DialogueManager.Instance.isDialogueActive)
            {
                DialogueManager.Instance.StartDialogue(dialogueSO);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
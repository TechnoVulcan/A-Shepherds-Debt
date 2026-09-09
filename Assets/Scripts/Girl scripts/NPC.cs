using UnityEngine;

public class NPC : MonoBehaviour
{
    public enum NPCState { Idle, Talk }
    public NPCState currentState = NPCState.Idle;

    [Header("Components")]
    public NPCTalk talk;               // Reference to NPCTalk
    public GameObject interactIcon;     // Reference to InteractIcon child object
    public Animator girlAnimator;       // Reference to sprite's Animator component

    [Header("Animation Trigger Names")]
    public string facePlayerAnimName = "face player"; // Name of animation or parameter
    public string idleAnimName = "Girl Idle";

    void Start()
    {
        // Hide interact icon at start
        if (interactIcon != null)
            interactIcon.SetActive(false);

        SwitchState(NPCState.Idle);
    }

    public void SwitchState(NPCState newState)
    {
        currentState = newState;

        if (newState == NPCState.Talk)
        {
            // Show icon
            if (interactIcon != null) 
                interactIcon.SetActive(true);

            // Enable Talk script
            if (talk != null) 
                talk.enabled = true;

            // Play Face Player animation
            if (girlAnimator != null) 
                girlAnimator.Play(facePlayerAnimName);
        }
        else // Idle State
        {
            // Hide icon
            if (interactIcon != null) 
                interactIcon.SetActive(false);

            // Disable Talk script
            if (talk != null) 
                talk.enabled = false;

            // Return to Idle animation
            if (girlAnimator != null) 
                girlAnimator.Play(idleAnimName);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SwitchState(NPCState.Talk);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SwitchState(NPCState.Idle);
        }
    }
}
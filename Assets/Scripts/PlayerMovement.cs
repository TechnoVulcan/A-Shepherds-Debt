using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5;
    public Rigidbody2D rb;
    public Animator anim;

    [Header("Footsteps")]
    [SerializeField] private AudioSource footstepSource;

    void FixedUpdate()
    {
        // Freeze movement while dialogue is active
        if (DialogueManager.Instance != null &&
            DialogueManager.Instance.isDialogueActive)
        {
            rb.linearVelocity = Vector2.zero;

            anim.SetFloat("horizontal", 0);
            anim.SetFloat("vertical", 0);

            if (footstepSource.isPlaying)
                footstepSource.Stop();

            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        anim.SetFloat("horizontal", horizontal);
        anim.SetFloat("vertical", vertical);

        rb.linearVelocity = new Vector2(horizontal, vertical) * speed;

        // Footstep audio
        bool isMoving = Mathf.Abs(horizontal) > 0.01f || Mathf.Abs(vertical) > 0.01f;

        if (isMoving)
        {
            if (!footstepSource.isPlaying)
                footstepSource.Play();
        }
        else
        {
            if (footstepSource.isPlaying)
                footstepSource.Stop();
        }
    }
}
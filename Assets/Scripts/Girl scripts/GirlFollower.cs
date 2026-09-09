using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GirlFollower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;

    [Header("Girl Light")]
    [SerializeField] private Light2D girlLight;

    [Header("Maze Teleport")]
    [SerializeField] private Transform mazeTeleportPoint;

    [Header("Point 1 Path")]
    [SerializeField] private Transform[] point1Path;

    [Header("Point 2 Path")]
    [SerializeField] private Transform[] point2Path;

    [Header("Point 3 Path")]
    [SerializeField] private Transform[] point3Path;

    [Header("Point 4 Path")]
    [SerializeField] private Transform[] point4Path;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.5f;

    [Header("Player Detection")]
    [SerializeField] private float playerReachDistance = 1.0f;

    private int currentPoint = 0;
    private int currentPathIndex = 0;

    private bool sequenceStarted = false;
    private bool waitingForPlayer = false;

    private Transform currentTarget;

    private void Start()
    {
        StopMoving();

        // Make sure the light starts OFF
        if (girlLight != null)
        {
            girlLight.enabled = false;
        }
    }

    private void FixedUpdate()
    {
        // Don't start moving until the girl dialogue is finished
        if (!GameProgress.Instance.talkedToGirl)
        {
            StopMoving();
            return;
        }

        // ------------------------------------------------
        // START FOLLOW SEQUENCE
        // ------------------------------------------------

        if (!sequenceStarted)
        {
            sequenceStarted = true;

            // Turn on girl's light
            if (girlLight != null)
            {
                girlLight.enabled = true;
            }

            currentPoint = 1;
            currentPathIndex = 0;

            SetCurrentTarget();
        }

        if (currentTarget == null || rb == null)
        {
            StopMoving();
            return;
        }

        // ------------------------------------------------
        // GIRL IS WAITING FOR PLAYER
        // ------------------------------------------------

        if (waitingForPlayer)
        {
            StopMoving();

            if (player != null)
            {
                float distanceToPlayer =
                    Vector2.Distance(
                        transform.position,
                        player.position
                    );

                if (distanceToPlayer <= playerReachDistance)
                {
                    MoveToNextPoint();
                }
            }

            return;
        }

        // ------------------------------------------------
        // MOVE TOWARDS CURRENT PATH NODE
        // ------------------------------------------------

        Vector2 target = currentTarget.position;
        Vector2 direction = target - rb.position;

        // Reached current path node
        if (direction.magnitude <= 0.1f)
        {
            StopMoving();

            MoveToNextPathNode();

            return;
        }

        direction.Normalize();

        UpdateAnimation(direction);

        rb.MovePosition(
            rb.position +
            direction *
            moveSpeed *
            Time.fixedDeltaTime
        );

        if (anim != null)
        {
            anim.SetBool("isWalking", true);
        }
    }

    // ====================================================
    // PATH MANAGEMENT
    // ====================================================

    private void MoveToNextPathNode()
    {
        currentPathIndex++;

        Transform[] currentPath = GetCurrentPath();

        if (currentPath == null || currentPath.Length == 0)
        {
            return;
        }

        // Still has path nodes
        if (currentPathIndex < currentPath.Length)
        {
            SetCurrentTarget();
            return;
        }

        // Reached end of current point
        currentPathIndex = currentPath.Length - 1;

        StopMoving();

        // Wait for player to catch up
        waitingForPlayer = true;
    }

    // ====================================================
    // MOVE TO NEXT POINT
    // ====================================================

    private void MoveToNextPoint()
    {
        waitingForPlayer = false;

        currentPoint++;
        currentPathIndex = 0;

        // ------------------------------------------------
        // AFTER POINT 4
        // ------------------------------------------------

        if (currentPoint > 4)
        {
            TeleportToMazeEnd();
            return;
        }

        SetCurrentTarget();
    }

    // ====================================================
    // TELEPORT GIRL TO END OF MAZE
    // ====================================================

    private void TeleportToMazeEnd()
    {
        StopMoving();

        if (mazeTeleportPoint == null)
        {
            Debug.LogWarning(
                "GirlFollower: Maze Teleport Point is not assigned!"
            );

            return;
        }

        // Teleport using Rigidbody2D position
        rb.position = mazeTeleportPoint.position;

        // Reset movement state
        currentTarget = null;
        waitingForPlayer = false;

        Debug.Log("Girl teleported to the end of the maze.");
    }

    // ====================================================
    // GET CURRENT PATH
    // ====================================================

    private Transform[] GetCurrentPath()
    {
        switch (currentPoint)
        {
            case 1:
                return point1Path;

            case 2:
                return point2Path;

            case 3:
                return point3Path;

            case 4:
                return point4Path;

            default:
                return null;
        }
    }

    // ====================================================
    // SET CURRENT TARGET
    // ====================================================

    private void SetCurrentTarget()
    {
        Transform[] currentPath = GetCurrentPath();

        if (currentPath == null ||
            currentPath.Length == 0)
        {
            currentTarget = null;
            return;
        }

        if (currentPathIndex >= currentPath.Length)
        {
            currentPathIndex =
                currentPath.Length - 1;
        }

        currentTarget =
            currentPath[currentPathIndex];
    }

    // ====================================================
    // ANIMATION
    // ====================================================

    private void UpdateAnimation(Vector2 direction)
    {
        if (anim == null)
            return;

        if (Mathf.Abs(direction.y) > 0.1f)
        {
            anim.SetBool(
                "facingUp",
                direction.y > 0
            );
        }
    }

    // ====================================================
    // STOP MOVING
    // ====================================================

    private void StopMoving()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (anim != null)
        {
            anim.SetBool("isWalking", false);
        }
    }
}
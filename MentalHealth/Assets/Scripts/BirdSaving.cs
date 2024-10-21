using UnityEngine;

public class BirdSaving : MonoBehaviour
{
    public Transform player;
    public BirdFollower birdFollower;
    public LayerMask platformLayer; 
    public float followDistance = 3f; 
    public float liftSpeed = 3f; 
    public float platformCheckRadius = 2f;
    public bool hasSavedPlayer = false;

    private Vector2 deathPosition;
    private bool isSaving = false;

    private Collider2D playerCollider;

    private bool isReturning = false; 
    private Vector2 targetFollowPosition;


    void Start()
    {
        playerCollider = player.GetComponent<Collider2D>();
    }

    void Update()
    {
        if (isSaving)
        {
            player.GetComponent<PlayerMovement>().enabled = false;

            // Move the bird to the player
            Vector2 birdPositionAbovePlayer = new Vector2(player.position.x, player.position.y + 1f);
            birdFollower.transform.position = Vector2.MoveTowards(birdFollower.transform.position, birdPositionAbovePlayer, liftSpeed * Time.deltaTime);

            // When the bird reaches the player, lift the player to the nearest platform
            if (Vector2.Distance(birdFollower.transform.position, birdPositionAbovePlayer) < 0.1f)
            {
                Transform nearestPlatform = FindNearestPlatform();
                if (nearestPlatform != null)
                {
                    // Calculate the platform's top position and player's height
                    float platformTopY = nearestPlatform.position.y + nearestPlatform.GetComponent<Collider2D>().bounds.extents.y;
                    float playerHeight = playerCollider.bounds.extents.y;

                    // Move the player to the platform's top position, slightly above to prevent collision
                    Vector2 targetHeight = new Vector2(player.position.x, platformTopY + playerHeight + 0.1f);
                    Vector2 targetPosition = new Vector2(nearestPlatform.position.x, platformTopY + playerHeight + 0.1f);
                    
                    player.position = Vector2.MoveTowards(player.position, targetHeight, liftSpeed * Time.deltaTime);
                    player.position = Vector2.MoveTowards(player.position, targetPosition, liftSpeed * Time.deltaTime);

                    // If the player has reached the platform, resume normal behavior
                    if (Vector2.Distance(player.position, targetPosition) < 0.1f)
                    {
                        ResumeNormal();
                    }
                }
            }
        }

        if (isReturning)
        {
            birdFollower.transform.position = Vector2.MoveTowards(birdFollower.transform.position, targetFollowPosition, liftSpeed * Time.deltaTime);

            if (Vector2.Distance(birdFollower.transform.position, targetFollowPosition) < 0.1f)
            {
                isReturning = false;
            }
        }
    }

    // Trigger this when the player dies
    public void PlayerDied(Vector2 playerDeathPosition)
    {
        if (!hasSavedPlayer)
        {
            deathPosition = playerDeathPosition;
            isSaving = true;
            hasSavedPlayer = true;
        }
    }

    // Find the nearest platform to the player
    private Transform FindNearestPlatform()
    {
        Collider2D[] hitPlatforms = Physics2D.OverlapCircleAll(player.position, platformCheckRadius, platformLayer);

        Transform nearestPlatform = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Collider2D platform in hitPlatforms)
        {
            float distanceToPlatform = Vector2.Distance(player.position, platform.transform.position);
            if (distanceToPlatform < shortestDistance)
            {
                shortestDistance = distanceToPlatform;
                nearestPlatform = platform.transform;
            }
        }

        return nearestPlatform;
    }

    // Resume normal behavior after saving
    private void ResumeNormal()
    {
        isSaving = false;
        isReturning = true;
        targetFollowPosition = new Vector2(player.position.x - followDistance, player.position.y);
        player.GetComponent<PlayerMovement>().enabled = true;
    }

}
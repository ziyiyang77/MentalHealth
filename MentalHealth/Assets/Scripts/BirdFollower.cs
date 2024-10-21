using UnityEngine;

public class BirdFollower : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 2f;
    public float distanceThreshold = 2f;

    private bool isFacingRight = true; // Track which direction the bird is facing

    void Update()
    {
        if (player != null) 
        {
            float distance = Vector2.Distance(transform.position, player.position);

            // Move the bird toward the player if the distance is greater than the threshold
            if (distance > distanceThreshold)
            {
                Vector2 direction = (player.position - transform.position).normalized;
                transform.position = Vector2.MoveTowards(transform.position, player.position, followSpeed * Time.deltaTime);

                // Flip the bird based on the direction it is moving
                if (direction.x > 0 && !isFacingRight)
                {
                    Flip();
                }
                else if (direction.x < 0 && isFacingRight)
                {
                    Flip();
                }
            }
        }

    }

    // Flip the bird's scale on the X-axis
    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}

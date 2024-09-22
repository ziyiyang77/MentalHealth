using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rushSpeed = 5f;
    public float energy = 100f;
    public Slider energySlider;
    public float energyConsumptionRate = 20f;
    public float energyRechargeRate = 10f;
    public float sinkingSpeed = 2f;
    public float idleTimeToSink = 3f;
    public float tiltSpeed = 5f;
    private Rigidbody2D rb;

    private float moveInput;
    private bool isFacingRight = true;
    private bool isOnReef = false;
    private float idleTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        bool isPressingW = Input.GetKey(KeyCode.W);

        if (moveInput > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && isFacingRight)
        {
            Flip();
        }

        // Rush
        if (isPressingW && energy > 0)
        {
            RushUpward();
            rb.drag = 0f;
        }
        else
        {
            // Increase drag to stop the player more quickly
            rb.drag = 3f;
        }

        // Energy
        if (isOnReef)
        {
            RechargeEnergy();
        }

        energySlider.value = energy/100;

        // Tilting behavior
        Quaternion targetRotation;

        if (isPressingW && moveInput != 0)
        {
            if (moveInput > 0)
            {
                targetRotation = Quaternion.Euler(0, 0, -45f);
            }
            else if (moveInput < 0)
            {
                targetRotation = Quaternion.Euler(0, 0, 45f);
            }
            else
            {
                targetRotation = Quaternion.Euler(0, 0, 0);
            }
        }
        else
        {
            targetRotation = Quaternion.Euler(0, 0, 0);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * tiltSpeed);

        // Sink
        if (moveInput != 0)
        {
            idleTimer = 0f;

            if (!isPressingW)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0f);
            }

        }
        else
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= idleTimeToSink && !isPressingW)
            {
                Sink();
            }
        }

    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void Sink()
    {
        rb.velocity = new Vector2(rb.velocity.x, -sinkingSpeed);

        if (transform.position.y < -Camera.main.orthographicSize)
        {
            Die();
        }
    }

    void RushUpward()
    {
        idleTimer = 0f;

        if (rb.velocity.y < 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
        }

        // Apply upward rush force
        rb.AddForce(new Vector2(0f, rushSpeed * Time.deltaTime), ForceMode2D.Impulse);

        // Cap the upward velocity to prevent excessive height
        if (rb.velocity.y > 5f)  // Maximum velocity
        {
            rb.velocity = new Vector2(rb.velocity.x, 5f);
        }

        energy -= energyConsumptionRate * Time.deltaTime;

        if (energy < 0)
        {
            energy = 0;
        }
    }

    void RechargeEnergy()
    {
        energy += energyRechargeRate * Time.deltaTime;

        if (energy > 100f)
        {
            energy = 100f;
        }
    }

    void Die()
    {
        Debug.Log("Player die.");
        Destroy(gameObject);
    }

    // Detect collision with reef
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Reef")
        {
            isOnReef = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Reef")
        {
            isOnReef = false;
        }
    }
}

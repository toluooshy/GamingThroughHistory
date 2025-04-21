using UnityEngine;

public class BallController : MonoBehaviour
{
    public int homeScore = 0;
    public int awayScore = 0;

    public GameObject myGoal;
    public GameObject rivalGoal;

    private Rigidbody rb;
    private Vector2 swipeStartPos;
    private float swipeStartTime;
    private bool isSwipeActive = false;

    public float swipeMinZ = -4f;
    public float swipeForce = 10f;
    public float maxSwipeDuration = 0.75f;
    public float maxForceMagnitude = 1f;

    [Header("Swipe Neighborhood")]
    public float swipeProximityThreshold = 100f; // in pixels

    // Temporary velocity freeze for smoother swipes
    private float velocityFreezeTimer = 0f;
    private const float freezeDuration = 0.1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (velocityFreezeTimer > 0f)
        {
            velocityFreezeTimer -= Time.deltaTime;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

    }

}

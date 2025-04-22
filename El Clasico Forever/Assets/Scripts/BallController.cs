using UnityEngine;

public class BallController : MonoBehaviour
{
    public GameObject GameController;

    public GameObject homeGoal;
    public GameObject awayGoal;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == homeGoal)
        {
            // Away team scores
            GameController.GetComponent<GameController>().awayScore++;
            Debug.Log("Away Team Scores! Total: " + GameController.GetComponent<GameController>().awayScore);
            ResetBall();
        }
        else if (other.gameObject == awayGoal)
        {
            // Home team scores
            GameController.GetComponent<GameController>().homeScore++;
            Debug.Log("Home Team Scores! Total: " + GameController.GetComponent<GameController>().homeScore);
            ResetBall();
        }
    }

    private void ResetBall()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = new Vector3(0, 1, 0);
    }
}

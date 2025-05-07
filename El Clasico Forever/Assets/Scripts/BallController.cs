using Photon.Pun;
using UnityEngine;

public class BallController : MonoBehaviourPunCallbacks
{
    public GameObject homeGoal;
    public GameObject awayGoal;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Only the master client controls the ball's clamped movement
        if (PhotonNetwork.IsMasterClient)
        {
            Vector3 pos = transform.position;

            if (pos.x < -3.6f)
            {
                pos.x = -2.5f;
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, rb.linearVelocity.z);
                transform.position = pos;
            }
            else if (pos.x > 3.6f)
            {
                pos.x = 2.5f;
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, rb.linearVelocity.z);
                transform.position = pos;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        GameSceneController controller = GameObject.FindGameObjectWithTag("GameSceneController").GetComponent<GameSceneController>();

        if (controller.homeSkin == 1)
        {
            if (other.CompareTag("AwayGoal"))
            {
                controller.IncrementAwayScore();
                ResetBall();
            }
            else if (other.CompareTag("HomeGoal"))
            {
                controller.IncrementHomeScore();
                ResetBall();
            }
        }
        else
        {
            if (other.CompareTag("AwayGoal"))
            {
                controller.IncrementHomeScore();
                ResetBall();
            }
            else if (other.CompareTag("HomeGoal"))
            {
                controller.IncrementAwayScore();
                ResetBall();
            }
        }
    }

    private void ResetBall()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = new Vector3(0, 1, -0.25f);
    }
}

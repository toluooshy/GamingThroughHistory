using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    public float yPlane = 1.25f;
    public Material[] playerSkins = new Material[2];

    private Vector3 spawnPosition;
    private PhotonView view;
    private Rigidbody rb;

    private bool isCollidingWithBall = false;
    private float stopTime = 0.5f; // Time to stop movement when colliding with ball
    private float stopTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        view = GetComponent<PhotonView>();

        // Apply skin from instantiation data
        if (view.InstantiationData != null && view.InstantiationData.Length > 0)
        {
            int skinIndex = (int)view.InstantiationData[0];
            if (TryGetComponent(out Renderer rend))
            {
                rend.material = playerSkins[skinIndex];
            }
        }

        if (view.IsMine)
        {
            spawnPosition = transform.position;
            SetupCamera();
        }
    }

    void Update()
    {
        if (!view.IsMine) return;

        if (isCollidingWithBall)
        {
            // Stop movement temporarily when colliding with the ball
            stopTimer += Time.deltaTime;
            if (stopTimer >= stopTime)
            {
                isCollidingWithBall = false;
                stopTimer = 0f;
            }
            return; // Skip movement if colliding with ball
        }

        // Handle movement when not colliding with ball
        if (Input.GetMouseButton(0))
        {
            Plane movementPlane = new Plane(Vector3.up, new Vector3(0, yPlane, 0));
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (movementPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                Vector3 clampedTargetPos = new Vector3(
                    Mathf.Clamp(hitPoint.x, spawnPosition.x - 2.4f, spawnPosition.x + 2.4f),
                    yPlane,
                    Mathf.Clamp(hitPoint.z, spawnPosition.z < 0 ? -14f : 1, spawnPosition.z < 0 ? -1 : 14f)
                );

                rb.MovePosition(clampedTargetPos);
            }
        }
    }

    private void SetupCamera()
    {
        Camera mainCam = Camera.main;

        if (!mainCam) return;

        float zDirection = Mathf.Sign(spawnPosition.z);
        Vector3 camOffset = new Vector3(0f, 10f, zDirection * 11f);

        mainCam.transform.position = spawnPosition + camOffset;
        mainCam.transform.rotation = Quaternion.Euler(25f, zDirection == 1 ? 180f : 0f, 0f);
    }
}

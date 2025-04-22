using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    public float yPlane = 1.25f;

    private Vector3 spawnPosition;
    private PhotonView view;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        view = GetComponent<PhotonView>();

        if (view.IsMine)
        {
            spawnPosition = transform.position;
            SetupCamera();
        }
    }

    void Update()
    {
        if (!view.IsMine) return;

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

        Vector3 camOffset;
        float zDirection = Mathf.Sign(spawnPosition.z);

        camOffset = new Vector3(0f, 10f, zDirection * 11f); // Keep camera behind player
        mainCam.transform.position = spawnPosition + camOffset;

        float xAngle = 25f;
        mainCam.transform.rotation = Quaternion.Euler(xAngle, zDirection == 1 ? 180f : 0f, 0f);
    }
}

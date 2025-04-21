using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    public float yPlane = 1f; // Fixed Y level the player moves on
    public float worldUnitsPerPixel = 0.01f; // Conversion scale from screen pixels to world units

    private Vector3 inputStartWorldPos;
    private Vector2 inputStartScreenPos;
    private bool isDragging = false;

    PhotonView view;

    void Start()
    {
        view = GetComponent<PhotonView>();
    }

    void Update()
    {
        if(view.IsMine) {
            if (Input.GetMouseButtonDown(0))
            {
                inputStartScreenPos = Input.mousePosition;
                inputStartWorldPos = transform.position;
                isDragging = true;
            }

            if (Input.GetMouseButton(0) && isDragging)
            {
                Vector2 currentScreenPos = Input.mousePosition;
                Vector2 screenDelta = currentScreenPos - inputStartScreenPos;

                // Convert screen delta to world space movement on XZ plane
                Vector3 worldDelta = new Vector3(screenDelta.x, 0f, screenDelta.y) * worldUnitsPerPixel;

                Vector3 targetPos = inputStartWorldPos;
                if ((targetPos + worldDelta).x > -3 && (targetPos + worldDelta).x < 3 && (targetPos + worldDelta).z < 0)
                {
                    targetPos += worldDelta;
                }
                targetPos.y = yPlane;

                transform.position = targetPos;
            }

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }
        }
    }
}

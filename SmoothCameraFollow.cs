using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    public Transform fpvTarget;
    public Transform tpvTarget;

    [Header("Takip Ayarları")]
    public float followSmoothTime = 0.3f;
    public float rotationSmoothSpeed = 5f;

    [Header("TPV Kamera Ayarları")]
    public float tpvDistance = 5f;
    public float tpvHeight = 2f;
    public float zoomSpeed = 5f;
    public float minDistance = 2f;
    public float maxDistance = 15f;

    private Vector3 velocity = Vector3.zero;
    private bool isFPV = false;

    void Update()
    {
        // Mod geçişi
        if (Input.GetKeyDown(KeyCode.C))
        {
            isFPV = !isFPV;
        }

        // Zoom kontrolü (sadece TPV modda)
        if (!isFPV)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            tpvDistance = Mathf.Clamp(tpvDistance - scroll * zoomSpeed, minDistance, maxDistance);
        }
    }

    void LateUpdate()
    {
        if (isFPV && fpvTarget != null)
        {
            // FPV → Kamera direkt sabitlenir
            transform.position = Vector3.SmoothDamp(transform.position, fpvTarget.position, ref velocity, followSmoothTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, fpvTarget.rotation, Time.deltaTime * rotationSmoothSpeed);
        }
        else if (tpvTarget != null)
        {
            // TPV → Uzak bir noktadan yumuşak takip
            Vector3 desiredPos = tpvTarget.position - tpvTarget.forward * tpvDistance + Vector3.up * tpvHeight;
            transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, followSmoothTime);

            Vector3 dir = tpvTarget.position - transform.position;
            Quaternion targetRot = Quaternion.LookRotation(dir.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSmoothSpeed);
        }
    }
}

using UnityEngine;
using System.Collections; // Coroutine için

public class DroneSettings : MonoBehaviour
{
    [Header("Cameras")]
    public GameObject camera1;
    public GameObject camera2;
    public GameObject þase;

    private Quaternion cameraTargetRotation;
    public float cameraSmoothSpeed = 5f;

    [Header("Gameplay Settings")]
    public bool kamikaze = false;
    public bool ammo = false;

    [Header("Ammo & Bomb")]
    public GameObject ammoIndicator;
    public GameObject bombPrefab;
    public Transform bombSpawnPoint;

    [Header("Explosion Settings")]
    public GameObject explosionEffectPrefab;
    public Vector3 targetPosition; // Patlamadan sonra ýþýnlanacaðý yer

    [Header("External References")]
    public DataTester dataTester;
    public CanvasController canvasController;

    private Rigidbody droneRb;
    private float defaultMass = 1.0f;
    private bool hasDropped = false;
    private bool isTouchingAmmoZone = false;

    private bool isRespawning = false;

    void Start()
    {
        droneRb = GetComponent<Rigidbody>();
        if (droneRb != null)
            defaultMass = droneRb.mass;

        if (kamikaze)
        {
            ammo = true;
            droneRb.mass = 1.5f;
            hasDropped = false;
        }
    }

    void Update()
    {
        if (dataTester == null || droneRb == null || isRespawning) return;

        HandleCameraSwitch();
        HandleCameraTilt();
        HandleAmmoPickup();
        HandleBombDrop();
        UpdateAmmoIndicator();
        ShowWeightWarning();
    }

    private void HandleCameraSwitch()
    {
        if (dataTester.sw1 == 2000)
        {
            camera1.SetActive(false);
            camera2.SetActive(true);
        }
        else if (dataTester.sw1 == 1000)
        {
            camera1.SetActive(true);
            camera2.SetActive(false);
        }
    }

    private void HandleCameraTilt()
    {
        if (camera2.activeSelf)
        {
            float tiltX = dataTester.rot1;
            float targetX = (tiltX / 10f) - 100f;

            // Hedef rotasyonu oluþtur
            cameraTargetRotation = Quaternion.Euler(targetX, 0f, 0f);

            // Kamerayý yumuþakça hedefe çevir
            camera2.transform.localRotation = Quaternion.Lerp(
                camera2.transform.localRotation,
                cameraTargetRotation,
                Time.deltaTime * cameraSmoothSpeed
            );
        }
    }


    private void HandleAmmoPickup()
    {
        if (!kamikaze && isTouchingAmmoZone && dataTester.sw2 == 2000)
        {
            ammo = true;
            droneRb.mass = 1.5f;
            hasDropped = false;
            canvasController?.ShowWarning("Ammo Acquired");
        }
    }

    private void HandleBombDrop()
    {
        if (ammo && dataTester.sw2 == 1000 && !hasDropped)
        {
            ammo = kamikaze;
            droneRb.mass = kamikaze ? 1.5f : defaultMass;
            hasDropped = !kamikaze;

            if (bombPrefab != null && bombSpawnPoint != null)
            {
                GameObject bomb = Instantiate(bombPrefab, bombSpawnPoint.position, bombSpawnPoint.rotation);
                Rigidbody bombRb = bomb.GetComponent<Rigidbody>();
                if (bombRb != null)
                    bombRb.velocity = droneRb.velocity;

                canvasController?.ShowWarning("Bomb Dropped");
            }
        }
    }

    private void UpdateAmmoIndicator()
    {
        if (ammoIndicator != null)
        {
            ammoIndicator.SetActive(ammo);
        }
    }

    private void ShowWeightWarning()
    {
        if (droneRb.mass > defaultMass)
        {
            canvasController?.ShowWarning("Weight Increase Detected");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ammo"))
        {
            isTouchingAmmoZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ammo"))
        {
            isTouchingAmmoZone = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (kamikaze && !collision.gameObject.CompareTag("kutu") && !isRespawning)
        {
           GameObject efekt = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            Destroy(efekt.gameObject, 3.5f);
            StartCoroutine(RespawnAfterDelay(2f));
            þase.gameObject.SetActive(false);
        }
    }

    private IEnumerator RespawnAfterDelay(float delay)
    {
        isRespawning = true;

        // Drone'u görünmez yap ve iþlevini kapat
        SetDroneActive(false);

        yield return new WaitForSeconds(delay);

        // Drone'u hedef konuma ýþýnla
        transform.position = targetPosition;
        droneRb.velocity = Vector3.zero;
        droneRb.angularVelocity = Vector3.zero;

        // Aktif hale getir
        SetDroneActive(true);
        isRespawning = false;
    }

    private void SetDroneActive(bool state)
    {
        þase.gameObject.SetActive(true);
       

        if (ammoIndicator != null) ammoIndicator.SetActive(state);

        // Rotasyonu sýfýrla
        if (!state)
        {
            transform.rotation = Quaternion.identity;
        }

        // KAMERA ve SCRIPT devre dýþý býrakýlmaz!
    }


}

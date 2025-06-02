using UnityEngine;

public class KamikazeControl : MonoBehaviour
{
    [Header("Drone Respawn Settings")]
    public GameObject dronePrefab;       // Yedekten çaðrýlacak drone prefab
    public Vector3 startPosition;        // Baþlangýç pozisyonu

    private GameObject mainDrone;        // "Main" adlý obje referansý

    void Start()
    {
        FindOrSpawnDrone();
    }

    void Update()
    {
        // Eðer Main drone sahnede yoksa tekrar oluþtur
        if (mainDrone == null)
        {
            FindOrSpawnDrone();
        }
    }

    private void FindOrSpawnDrone()
    {
        // Sahnede "Main" adlý obje var mý diye kontrol et
        GameObject found = GameObject.Find("Main");

        if (found != null)
        {
            mainDrone = found;
        }
        else if (dronePrefab != null)
        {
            // Yeni drone oluþtur ve adýný "Main" yap
            GameObject newDrone = Instantiate(dronePrefab, startPosition, Quaternion.identity);
            newDrone.name = "Main";
            mainDrone = newDrone;
        }
    }
}

using UnityEngine;

public class KamikazeControl : MonoBehaviour
{
    [Header("Drone Respawn Settings")]
    public GameObject dronePrefab;       // Yedekten �a�r�lacak drone prefab
    public Vector3 startPosition;        // Ba�lang�� pozisyonu

    private GameObject mainDrone;        // "Main" adl� obje referans�

    void Start()
    {
        FindOrSpawnDrone();
    }

    void Update()
    {
        // E�er Main drone sahnede yoksa tekrar olu�tur
        if (mainDrone == null)
        {
            FindOrSpawnDrone();
        }
    }

    private void FindOrSpawnDrone()
    {
        // Sahnede "Main" adl� obje var m� diye kontrol et
        GameObject found = GameObject.Find("Main");

        if (found != null)
        {
            mainDrone = found;
        }
        else if (dronePrefab != null)
        {
            // Yeni drone olu�tur ve ad�n� "Main" yap
            GameObject newDrone = Instantiate(dronePrefab, startPosition, Quaternion.identity);
            newDrone.name = "Main";
            mainDrone = newDrone;
        }
    }
}

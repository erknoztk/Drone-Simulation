using UnityEngine;

public class Portal : MonoBehaviour
{
    public int id; // Bu portala atanacak s�ra numaras� (1-6)

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Drone'un tag'� "Player" olmal�
        {
            DroneManager drone = other.GetComponent<DroneManager>();
            if (drone != null)
            {
                bool activated = drone.ActivatePortal(id);
                if (activated)
                {
                    Debug.Log("Portal " + id + " ba�ar�yla ge�ildi.");
                    Destroy(gameObject); // Portal sahneden yok edilir
                }
                else
                {
                    Debug.Log("Portal " + id + " s�ras� gelmedi.");
                }
            }
        }
    }
}

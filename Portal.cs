using UnityEngine;

public class Portal : MonoBehaviour
{
    public int id; // Bu portala atanacak sýra numarasý (1-6)

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Drone'un tag'ý "Player" olmalý
        {
            DroneManager drone = other.GetComponent<DroneManager>();
            if (drone != null)
            {
                bool activated = drone.ActivatePortal(id);
                if (activated)
                {
                    Debug.Log("Portal " + id + " baþarýyla geçildi.");
                    Destroy(gameObject); // Portal sahneden yok edilir
                }
                else
                {
                    Debug.Log("Portal " + id + " sýrasý gelmedi.");
                }
            }
        }
    }
}

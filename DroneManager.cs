using UnityEngine;

public class DroneManager : MonoBehaviour
{
    public bool[] portalPassed = new bool[7]; // 0–6 arasý portallar

    public bool ActivatePortal(int id)
    {
        // Dizi sýnýrlarý içinde mi kontrolü
        if (id < 0 || id >= portalPassed.Length)
        {
            Debug.LogWarning($"Invalid portal ID: {id}");
            return false;
        }

        portalPassed[id] = true;
        return true;
    }
}

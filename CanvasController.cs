using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CanvasController : MonoBehaviour
{
    public DroneController droneController;
    public GameObject droneObject;

    public Image motorFLBar;
    public Image motorFRBar;
    public Image motorRLBar;
    public Image motorRRBar;

    public TextMeshProUGUI amperText;
    public TextMeshProUGUI telemetryText;
    public TextMeshProUGUI warningText;

    public float maxWidth = 200f;
    private bool showingWarning = false;

    void Update()
    {
        if (droneController == null || droneObject == null) return;

        // === Motor barlarý ===
        SetBarWidth(motorFLBar, droneController.motorFL);
        SetBarWidth(motorFRBar, droneController.motorFR);
        SetBarWidth(motorRLBar, droneController.motorRL);
        SetBarWidth(motorRRBar, droneController.motorRR);

        // === Amper hesapla ===
        float avgPower = (droneController.motorFL + droneController.motorFR + droneController.motorRL + droneController.motorRR) / 4f;
        float amps = Mathf.Lerp(10f, 80f, avgPower);
        if (amperText != null) amperText.text = amps.ToString("F1") + " A";

        // === Telemetri ===
        float height = droneObject.transform.position.y;
        Vector3 rotation = droneObject.transform.eulerAngles;
        if (telemetryText != null)
        {
            telemetryText.text =
                "Height: " + height.ToString("F1") + " m\n" +
                "Roll:   " + rotation.z.ToString("F1") + "°\n" +
                "Pitch:  " + rotation.x.ToString("F1") + "°\n" +
                "Yaw:    " + rotation.y.ToString("F1") + "°";
        }

        // === Uyarý sistemi: terslik algýlama ===
        if (!showingWarning)
        {
            if (Vector3.Dot(droneObject.transform.up, Vector3.down) > 0.7f) // Drone neredeyse ters
            {
                ShowWarning("CRASH DETECTED!");
            }
            else if (Vector3.Dot(droneObject.transform.up, Vector3.up) < 0.3f) // Aþýrý eðik
            {
                ShowWarning("UNSTABLE FLIGHT!");
            }
        }
    }

    void SetBarWidth(Image barImage, float value)
    {
        if (barImage != null)
        {
            RectTransform rt = barImage.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(value * maxWidth, rt.sizeDelta.y);
        }
    }

    public void ShowWarning(string message)
    {
        if (warningText == null) return;
        StopAllCoroutines();
        StartCoroutine(ShowTemporaryWarning(message, 2f));
    }

    IEnumerator ShowTemporaryWarning(string message, float duration)
    {
        showingWarning = true;
        warningText.text = message;
        warningText.enabled = true;
        yield return new WaitForSeconds(duration);
        warningText.enabled = false;
        showingWarning = false;
    }
}

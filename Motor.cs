using UnityEngine;

public class Motor : MonoBehaviour
{
    public Transform rotorVisual;
    public bool clockwise = true;
    public float maxThrust = 10f;

    [Range(0f, 1f)] public float currentPower;

    public float minSpinSpeed = 300f;
    public float maxSpinSpeed = 3000f;

    public void ApplyThrust(Rigidbody rb)
    {
        rb.AddForceAtPosition(transform.up * currentPower * maxThrust, transform.position);
    }

    public void RotateVisual()
    {
        if (rotorVisual == null) return;
        float dir = clockwise ? 1f : -1f;
        float speed = Mathf.Lerp(minSpinSpeed, maxSpinSpeed, currentPower);
        rotorVisual.Rotate(Vector3.up * dir * speed * Time.deltaTime);
    }
}

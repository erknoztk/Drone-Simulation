using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DroneController : MonoBehaviour
{
    public DataTester inputSource; // Giriş verisi: kumandadan gelen throttle, yaw, roll, pitch
    public Motor[] motors; // 0: FL, 1: FR, 2: RL, 3: RR (Motor sıralaması: Front-Left, Front-Right, Rear-Left, Rear-Right)

    private Rigidbody rb;

    [Header("PID Stabilizasyon")]
    public float stabilizationStrength = 8f; // Drone düz durmaya çalışırken ne kadar sert tepki verir
    public float damping = 2f;               // Fazla açısal hızı bastırma (gyro frenlemesi)

    [Header("Motor Güç Katsayıları")]
    public float throttleStrength = 1f;  // Yukarı kaldırma gücü
    public float pitchFactor = 0.5f;     // İleri/Geri (Z)
    public float rollFactor = 0.5f;      // Sağ/Sol (X)
    public float yawFactor = 0.4f;       // Y ekseni etrafında dönme (yaw)

    [Header("Motor Güçleri (Canlı Görsel)")]
    [Range(0f, 1f)] public float motorFL;
    [Range(0f, 1f)] public float motorFR;
    [Range(0f, 1f)] public float motorRL;
    [Range(0f, 1f)] public float motorRR;

    public AudioSource droneAudio;

    public bool stabilize = false;
    private Vector3 targetUp;
    private Vector3 targetPosition;



    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = Vector3.zero;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        targetUp = transform.up;
        targetPosition = transform.position;


        if (motors.Length != 4)
            Debug.LogError("4 motor atanmalı! Sıralama: FL, FR, RL, RR");
    }

    void FixedUpdate()
    {
        // Stabilizasyon anahtarı
        stabilize = inputSource.sw4 >= 2000;
        if (inputSource == null) return;
        if (stabilize && targetPosition == Vector3.zero)
            targetPosition = transform.position;
        else if (!stabilize)
            targetPosition = Vector3.zero;  // stabil moddan çıkınca sıfırla

        // === 1. Giriş Eşlemeleri ===
        float throttle = 0f, forward = 0f, strafe = 0f, yaw = 0f;

        if (!stabilize)
        {
            throttle = Mathf.InverseLerp(1000, 2000, inputSource.pitch);
            forward = NormalizeCentered(inputSource.roll);
            strafe = NormalizeCentered(inputSource.throttle);
            yaw = NormalizeCentered(inputSource.yaw);
        }

        // === 2. PID Stabilizasyon ===
        Vector3 error = Vector3.Cross(transform.up, Vector3.up);
        Vector3 correctiveTorque = error * stabilizationStrength - rb.angularVelocity * damping;
        rb.AddTorque(correctiveTorque, ForceMode.Force);

        // === 3. Yaw için fiziksel tork uygula ===
        Vector3 yawTorque = Vector3.up * yaw * yawFactor * stabilizationStrength;
        rb.AddTorque(yawTorque, ForceMode.Force);

        // === 4. Motor Güç Karışımı Hesabı ===
        float basePower = throttle * throttleStrength;

        float[] mix = new float[4];
        mix[0] = basePower + forward * pitchFactor + strafe * rollFactor - yaw * yawFactor; // FL
        mix[1] = basePower + forward * pitchFactor - strafe * rollFactor + yaw * yawFactor; // FR
        mix[2] = basePower - forward * pitchFactor + strafe * rollFactor + yaw * yawFactor; // RL
        mix[3] = basePower - forward * pitchFactor - strafe * rollFactor - yaw * yawFactor; // RR

        // === 5. Motorlara Güç Ver + Görsel Döndürme ===
        for (int i = 0; i < 4; i++)
        {
            motors[i].currentPower = Mathf.Clamp01(mix[i]);
            motors[i].ApplyThrust(rb);
            motors[i].RotateVisual();
        }

        // === 6. Motor Güçlerini Inspector’da Göster ===
        motorFL = motors[0].currentPower;
        motorFR = motors[1].currentPower;
        motorRL = motors[2].currentPower;
        motorRR = motors[3].currentPower;

        // === 7. Drift Azaltma ===
        Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
        localVelocity.x = Mathf.Lerp(localVelocity.x, 0f, Time.fixedDeltaTime * 0.5f);
        localVelocity.z = Mathf.Lerp(localVelocity.z, 0f, Time.fixedDeltaTime * 0.5f);
        rb.velocity = transform.TransformDirection(localVelocity);

        if (droneAudio != null)
        {
            // Ortalama motor gücüne göre ses seviyesi ve pitch ayarla
            float avgPower = (motorFL + motorFR + motorRL + motorRR) / 4f;
            droneAudio.volume = Mathf.Lerp(0.1f, 1.0f, avgPower);   // Daha düşük güçte daha sessiz
            droneAudio.pitch = Mathf.Lerp(0.8f, 2.0f, avgPower);     // Güce göre daha tiz ses
        }

      
        // Hedef yukarı yön ve pozisyon

        if (stabilize)
        {
            // Yukarı sabitleme: Drone düz dursun
            Vector3 errorRate = Vector3.Cross(transform.up, Vector3.up);
            Vector3 correctiveTorq = errorRate * stabilizationStrength;

            // Pozisyon koruma: Yükseklik sabitlenmeye çalışılır
            float altitudeError = targetPosition.y - transform.position.y;
            float targetAltitudePower = 3f + altitudeError * 6f; // 5 çok fazlaydı
            float basePowerDrone = Mathf.Lerp(basePower, targetAltitudePower, Time.fixedDeltaTime * 3f);
            basePowerDrone = Mathf.Clamp01(basePowerDrone);

            // Pitch, Roll ve Yaw'ı bastırmak için gyro kullan
            Vector3 angVel = rb.angularVelocity;

            float pitch = -transform.right.y * stabilizationStrength - angVel.x * damping;
            float roll = -transform.forward.y * stabilizationStrength - angVel.z * damping;
            float yawControl = 0;

            float[] mix2 = new float[4];
            mix2[0] = basePowerDrone + pitch * pitchFactor + roll * rollFactor - yawControl * yawFactor; // FL
            mix2[1] = basePowerDrone + pitch * pitchFactor - roll * rollFactor + yawControl * yawFactor; // FR
            mix2[2] = basePowerDrone - pitch * pitchFactor + roll * rollFactor + yawControl * yawFactor; // RL
            mix2[3] = basePowerDrone - pitch * pitchFactor - roll * rollFactor - yawControl * yawFactor; // RR

            for (int i = 0; i < 4; i++)
            {
                motors[i].currentPower = Mathf.Clamp01(mix2[i]);
                motors[i].ApplyThrust(rb);
                motors[i].RotateVisual();
            }

            // Inspector için:
            motorFL = motors[0].currentPower;
            motorFR = motors[1].currentPower;
            motorRL = motors[2].currentPower;
            motorRR = motors[3].currentPower;

            return; // normal kontrolleri atla
        }

    }

    // === RC kanal değerlerini -1 ila 1 arasına çevirir ===
    float NormalizeCentered(int input)
    {
        return Mathf.Clamp((input - 1500) / 500f, -1f, 1f);
    }
}

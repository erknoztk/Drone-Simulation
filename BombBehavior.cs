using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BombBehavior : MonoBehaviour
{
    public Vector3 targetRotationEuler = new Vector3(90f, 0f, 0f);
    public float rotationLerpSpeed = 1f;
    public float extraGravityForce = 30f;

    public GameObject explosionEffect; // Patlama efekt prefab'ý
    public float explosionDestroyDelay = 3.5f; // Efektin sahneden silinme süresi

    private Quaternion targetRotation;
    private Rigidbody rb;

    void Start()
    {
        targetRotation = Quaternion.Euler(targetRotationEuler);
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Yavaþ yavaþ hedef rotasyona dön
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationLerpSpeed * Time.fixedDeltaTime);

        // Ekstra aþaðý çekme
        rb.AddForce(Vector3.down * extraGravityForce, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Player"))
        {
            if (explosionEffect != null)
            {
                GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
                Destroy(explosion, explosionDestroyDelay); // Efekti 3.5 saniye sonra yok et
            }

            Destroy(gameObject); // Bombayý yok et
        }
    }
}

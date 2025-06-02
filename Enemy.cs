using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public int health = 100;
    public float maxDamage = 100f;
    public float explosionRadius = 5f;

    public GameObject dieEffect;
    public Image healthBarImage;

    public bool isDead = false; // Þimdi public oldu ve dýþarýdan da eriþilebilir

    private int maxHealth;
    private float maxBarWidth = 97.9f;

    private void Start()
    {
        maxHealth = health;
        UpdateHealthBar();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Explosion"))
        {
            float distance = Vector3.Distance(transform.position, other.transform.position);

            if (distance <= explosionRadius)
            {
                float damageRatio = 1f - (distance / explosionRadius);
                int damage = Mathf.RoundToInt(maxDamage * damageRatio);
                ApplyDamage(damage);
            }
        }
    }

    private void ApplyDamage(int amount)
    {
        if (isDead) return;

        health -= amount;
        health = Mathf.Clamp(health, 0, maxHealth);

        Debug.Log($"Enemy hasar aldý: {amount} | Kalan can: {health}");

        UpdateHealthBar();

        if (health <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBarImage != null)
        {
            float ratio = (float)health / maxHealth;
            float newWidth = ratio * maxBarWidth;

            RectTransform rt = healthBarImage.rectTransform;
            rt.sizeDelta = new Vector2(newWidth, rt.sizeDelta.y);
        }
    }

    private void Die()
    {
        isDead = true;

        if (dieEffect != null)
        {
            dieEffect.SetActive(true);
        }

        Debug.Log("Enemy öldü, efekt aktif!");
    }
}

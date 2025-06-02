using UnityEngine;
using TMPro;

public class EnemyController : MonoBehaviour
{
    public TextMeshProUGUI infoText;

    [Header("Inspector Info")]
    public int totalEnemyCount;
    public int totalEnemyHealth;

    private float timer = 0f;
    private bool timerStarted = false;
    private bool timerActive = false;

    private void Start()
    {
        // 3 saniye sonra zamanlay�c�y� ba�lat
        Invoke(nameof(StartTimer), 3f);
    }

    private void Update()
    {
        UpdateEnemyStats();

        // Saya� �al���yorsa g�ncelle
        if (timerActive)
        {
            timer += Time.deltaTime;
        }
    }

    private void StartTimer()
    {
        timerStarted = true;
        timerActive = true;
    }

    private void UpdateEnemyStats()
    {
        Enemy[] allEnemies = FindObjectsOfType<Enemy>();

        totalEnemyCount = 0;
        totalEnemyHealth = 0;

        foreach (Enemy enemy in allEnemies)
        {
            if (enemy.isDead) continue;

            totalEnemyCount++;
            totalEnemyHealth += enemy.health;
        }

        // T�m d��manlar �ld�yse saya� dursun
        if (timerStarted && totalEnemyCount == 0)
        {
            timerActive = false;
        }

        // UI Text g�ncelle
        if (infoText != null)
        {
            string timerText = timerStarted ? $"\nTime: {timer:F2}s" : "\nTime: ...";
            infoText.text = $"Enemy Count: {totalEnemyCount}\nTotal Health: {totalEnemyHealth}{timerText}";
        }
    }
}

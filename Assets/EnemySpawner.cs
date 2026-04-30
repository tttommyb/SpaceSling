using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemy_prefab;
    [SerializeField] int enemy_count = 1;
    [SerializeField] TMP_Text stats_text;
    [SerializeField] TMP_Text progress_text;
    [SerializeField] Scrollbar progress_bar;
    [SerializeField] GameObject player;

    int kills = 0;
    int wave = 1;
    int last_inactive_count = 0; // Tracks deaths frame-by-frame

    void Start()
    {
        SpawnInitialEnemies();
    }

    void Update()
    {
        int current_inactive = 0;
        foreach (Transform t in transform)
        {
            if (!t.gameObject.activeSelf) current_inactive++;
        }

        // --- KILL LOGIC ---
        // If there are more inactive enemies now than there were last frame, 
        // it means enemies died. Add that difference to the kill count.
        if (current_inactive > last_inactive_count)
        {
            int newDeaths = current_inactive - last_inactive_count;
            kills += newDeaths;
            last_inactive_count = current_inactive;
        }

        // UI Updates
        int remaining = enemy_count - current_inactive;
        progress_bar.size = (float)remaining / enemy_count;
        progress_text.text = $"REMAINING: {remaining}";
        stats_text.text = $"Kills: {kills}\nWave: {wave}";

        // Wave Reset Logic
        if (remaining <= 0)
        {
            NextWave();
        }
    }

    void NextWave()
    {
        wave++;
        enemy_count++;
        last_inactive_count = 0; // Reset this so the new wave starts fresh

        if (wave % 5 == 0) player.GetComponent<Health>()?.AddLife();

        // Reactivate and reposition old enemies
        int i = 0;
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(true);
            t.position = player.transform.position + new Vector3(10.0f * i, 100.0f);
            t.GetComponent<Health>()?.AddLife();
            i++;
        }

        // Add the extra enemy for the new wave
        GameObject newEnemy = Instantiate(enemy_prefab, player.transform.position + new Vector3(10.0f * i, 100.0f), Quaternion.identity);
        newEnemy.transform.parent = this.transform;
    }

    void SpawnInitialEnemies()
    {
        for (int i = 0; i < enemy_count; i++)
        {
            GameObject e = Instantiate(enemy_prefab, player.transform.position + new Vector3(10.0f * i, 100.0f), Quaternion.identity);
            e.transform.parent = this.transform;
        }
    }
}
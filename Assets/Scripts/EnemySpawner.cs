
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] List<GameObject> enemy_prefabs = new List<GameObject>();
    [SerializeField] int enemy_count = 1;
    [SerializeField] TMP_Text stats_text;
    [SerializeField] GameObject player;
    [SerializeField] DynamicEnemyUI enemy_UI;

    int kills = 0;
    int wave = 1;
    int last_inactive_count = 0; // Tracks deaths frame-by-frame
    Dictionary<string, int> enemy_type_counts = new Dictionary<string, int>(); 
    void Start()
    {
        SpawnInitialEnemies();

        UpdateUI();
    }

    void Update()
    {
        
        int current_inactive = 0;
        foreach (Transform t in transform)
        {
            if (!t.gameObject.activeSelf) 
            {
                current_inactive++;
            }
        }

       

        // --- KILL LOGIC ---
        // If there are more inactive enemies now than there were last frame, 
        // it means enemies died. Add that difference to the kill count.
        if (current_inactive > last_inactive_count)
        {
            int newDeaths = current_inactive - last_inactive_count;
            kills += newDeaths;
            last_inactive_count = current_inactive;

            UpdateUI();

        }

        // Wave Reset Logic
        if (current_inactive == enemy_count)
        {
            NextWave();
        }
    }

    void NextWave()
    {
        wave++;
        enemy_count++;
        last_inactive_count = 0; // Reset this so the new wave starts fresh

        if (wave % 2 == 0) player.GetComponent<Health>()?.AddLife();

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
        GameObject newEnemy = Instantiate(enemy_prefabs[Random.Range(0, enemy_prefabs.Count)], player.transform.position + new Vector3(10.0f * i, 100.0f), Quaternion.identity);
        newEnemy.transform.parent = this.transform;

        UpdateUI();
    }

    void SpawnInitialEnemies()
    {
        for (int i = 0; i < enemy_count; i++)
        {
            GameObject e = Instantiate(enemy_prefabs[Random.Range(0, enemy_prefabs.Count)], player.transform.position + new Vector3(10.0f * i, 100.0f), Quaternion.identity);
            e.transform.parent = this.transform;
        }
    }

    void UpdateUI() 
    {

        enemy_type_counts.Clear();
        int unique_types = 0;
        foreach (Transform t in transform)
        {
            if (t.gameObject.activeSelf)
            {
                if (!enemy_type_counts.ContainsKey(t.tag))
                {
                    unique_types++;
                    enemy_type_counts.Add(t.tag, 0);
                }
                enemy_type_counts[t.tag]++;
            }
        }
        enemy_UI.RefreshUI(enemy_type_counts);
    }
}


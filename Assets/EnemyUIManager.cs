using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class DynamicEnemyUI : MonoBehaviour
{
    public GameObject row_prefab;     // Empty GO with Horizontal Layout Group (Centered)
    public GameObject icon_prefab;    // Your Enemy Icon
    public Transform container;      // The Master Vertical Layout Group
    public int max_icons_per_row = 5;

    private List<string> type_keys = new List<string>();

    int count = 0;

    public void RefreshUI(Dictionary<string, int> enemy_types)
    {
        type_keys.Clear();
        foreach (string key in enemy_types.Keys) type_keys.Add(key);

        int total_types = enemy_types.Count;
        // 1. Clear existing rows
        foreach (Transform child in container) Destroy(child.gameObject);

        int created_icons = 0;
        int rows_needed = Mathf.CeilToInt((float)total_types / max_icons_per_row);

        // 2. Adjust scaling based on row count
        float scale = Mathf.Clamp(1.2f - (rows_needed * 0.2f), 0.5f, 1.0f);

        for (int i = 0; i < rows_needed; i++)
        {
            // Create a new centered row
            GameObject currentRow = Instantiate(row_prefab, container);

            // Fill the row
            for (int j = 0; j < max_icons_per_row; j++)
            {
                if (created_icons >= total_types) break;

                string current_type = type_keys[created_icons];

                GameObject icon = Instantiate(icon_prefab, currentRow.transform);
                icon.tag = type_keys[created_icons];
                icon.transform.localScale = Vector3.one * scale;
                icon.SetActive(true);

                TextMeshProUGUI label = icon.GetComponentInChildren<TextMeshProUGUI>();
                if (label != null)
                {
                    label.text = enemy_types[current_type].ToString();
                }

                created_icons++;

            }
        }
    }
}
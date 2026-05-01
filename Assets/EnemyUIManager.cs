using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DynamicEnemyUI : MonoBehaviour
{
    public GameObject rowPrefab;     // Empty GO with Horizontal Layout Group (Centered)
    public GameObject iconPrefab;    // Your Enemy Icon
    public Transform container;      // The Master Vertical Layout Group
    public int maxIconsPerRow = 5;

    int count = 0;

    public void RefreshUI(int totalEnemies)
    {
        // 1. Clear existing rows
        foreach (Transform child in container) Destroy(child.gameObject);

        int createdIcons = 0;
        int rowsNeeded = Mathf.CeilToInt((float)totalEnemies / maxIconsPerRow);

        // 2. Adjust scaling based on row count
        float scale = Mathf.Clamp(1.2f - (rowsNeeded * 0.2f), 0.5f, 1.0f);

        for (int i = 0; i < rowsNeeded; i++)
        {
            // Create a new centered row
            GameObject currentRow = Instantiate(rowPrefab, container);

            // Fill the row
            for (int j = 0; j < maxIconsPerRow; j++)
            {
                if (createdIcons >= totalEnemies) break;

                GameObject icon = Instantiate(iconPrefab, currentRow.transform);
                icon.transform.localScale = Vector3.one * scale;
                createdIcons++;
            }
        }
    }
}
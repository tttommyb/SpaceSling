using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemy_prefab;
    [SerializeField] int enemy_count = 1;
    // Start is called before the first frame update
    void Start()
    {
            for(int i = 0; i < enemy_count; i++) 
            {
                Instantiate(enemy_prefab, this.transform.position + new Vector3(10.0f * i, 0.0f), Quaternion.identity);
            }
        
    }

    // Update is called once per frame
    void Update()
    {
        int count = 0;
        foreach (Transform t in transform)
        {
            if (!t.gameObject.activeSelf) 
            {
                count++;
            }
        }
        if (count == enemy_count) 
        {
            foreach(Transform t in transform) 
            {
                t.gameObject.SetActive(true);
                t.transform.position = Vector3.zero + new Vector3(10.0f * count, 0.0f);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionTimer : MonoBehaviour
{
    [SerializeField] float life_time = 1.0f;
    float initial_life_time;
    // Start is called before the first frame update
    void Start()
    {
        initial_life_time = life_time;
    }

    // Update is called once per frame
    void Update()
    {
        life_time -= Time.deltaTime;
        float scale= life_time / initial_life_time;
        transform.localScale = new Vector3(scale * 2, scale * 2 , 1.0f);
        if(life_time <= 0.0f) 
        {
            Destroy(gameObject);
        }
    }
}

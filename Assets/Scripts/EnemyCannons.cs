using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCannons : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] GameObject target;

    [Header("Projectile Settings")]
    [SerializeField] GameObject projectile_prefab;
    [SerializeField] float reload_time;

    float reload_timer;

    Projectile projectile;

    Rigidbody2D rb;

    

    // Start is called before the first frame update
    void Start()
    {
        reload_timer = reload_time;
        target = GameObject.Find("Player");
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!target.activeSelf) return;
        if(reload_timer > 0) 
        {
            reload_timer -= Time.deltaTime;
        }
        if(reload_timer <= 0) 
        {
            reload_timer = reload_time;
            Vector2 velocity = rb.velocity;
            Vector2 target_pos = target.transform.position;
            Vector2 displacement = target_pos - (Vector2)transform.position;
            float dot = Vector2.Dot(velocity.normalized, displacement.normalized);
            if (dot > 0.99f && displacement.magnitude <= 40.0f)
            {
                projectile = Instantiate(projectile_prefab, transform.position, transform.rotation).GetComponent<Projectile>();
                projectile.Initialise(rb.velocity, LayerMask.GetMask("Player"), LayerMask.GetMask("Player", "Asteroid"), Color.yellow);
                Debug.Log("FIRE!");
                GetComponent<AudioSource>().Play();
            }
        }
       
    }
}

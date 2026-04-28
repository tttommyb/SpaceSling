using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy_Shooter_Movement : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] GameObject target;

    [Header("Movement")]
    [SerializeField] float move_speed = 15.0f;
    [SerializeField] float turn_speed = 1.0f;

    Vector2 target_position;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Player");
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        target_position = target.transform.position;
        // 1. Get the direction to the player
        Vector2 chase_dir = (target_position - (Vector2)transform.position).normalized;

        // 2. Get the separation vector (the sum of all 'push away' forces)
        Vector2 separation_dir = GetSeperationVector();

        // 3. Combine them (Weighted)
        // 1.0f weight for chasing, 1.5f for separation (don't hit friends!)
        Vector2 finalDesiredDir = (chase_dir * 1.4f) + (separation_dir * 1.5f);
        finalDesiredDir.Normalize();

        // 4. Rotate the ship toward the FINAL desired direction
        float targetAngle = Mathf.Atan2(finalDesiredDir.y, finalDesiredDir.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRot = Quaternion.Euler(0, 0, targetAngle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, turn_speed * Time.deltaTime);

        // 5. Move strictly FORWARD relative to the new rotation
        rb.velocity = transform.up * move_speed;
    }

    void LookAtTarget()
    {
        
        Vector2 direction = rb.velocity;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    Vector2 GetSeperationVector()
    {
        //Seperation
        Collider2D[] nearby_enemies = Physics2D.OverlapCircleAll(transform.position, 2.0f, LayerMask.GetMask("Enemy"));
        Vector2 total_dir = Vector2.zero;
        int count = 0;
        foreach(var enemy  in nearby_enemies) 
        {
            if (enemy.gameObject == gameObject) continue;
            Debug.DrawLine(transform.position, enemy.transform.position, Color.red);
            Vector2 displacement = (transform.position - enemy.transform.position);
            total_dir += displacement.normalized / displacement.magnitude;  
            count++;
        }
        if (count > 0) 
        {
            return ((total_dir/count).normalized);
        }
        return Vector2.zero;

    }

}

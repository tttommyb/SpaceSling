using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyShooterMovement : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] GameObject target;

    [Header("Movement")]
    [SerializeField] float move_speed = 15.0f;
    [SerializeField] float default_turn_speed = 1.0f;

    float turn_speed = 1.0f;

    Boid boid;

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Player");
        rb = GetComponent<Rigidbody2D>();
        turn_speed = default_turn_speed;
        boid = GetComponent<Boid>();
    }

    void FixedUpdate()
    {
        Vector2 boid_dir = Vector2.zero;
        if (boid.enabled) 
        {
            boid_dir = boid.getDesiredDirection(target);
        }
        

        // 4. Rotate the ship toward the FINAL desired direction
        float targetAngle = Mathf.Atan2(boid_dir.y, boid_dir.x) * Mathf.Rad2Deg - 90f;
        Quaternion target_rot = Quaternion.Euler(0, 0, targetAngle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target_rot, turn_speed * Time.deltaTime);
        
        // 5. Move strictly FORWARD relative to the new rotation
        rb.velocity = transform.up * move_speed;
    }

}

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
    [SerializeField] float default_turn_speed = 1.0f;
    [SerializeField] float asteroid_clearance = 1.0f;

    float turn_speed = 1.0f;

    Vector2 target_position;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Player");
        rb = GetComponent<Rigidbody2D>();
        turn_speed = default_turn_speed;
    }

    void FixedUpdate()
    {
        target_position = target.transform.position;
        // 1. Get the direction to the player
        Vector2 chase_dir = (target_position - (Vector2)transform.position).normalized;

        Collider2D[] nearby_enemies = Physics2D.OverlapCircleAll(transform.position, 2.0f, LayerMask.GetMask("Enemy"));

        // 2. Get the separation vector (the sum of all 'push away' forces)
        Vector2 separation_dir = GetSeperationVector(nearby_enemies);
        Vector2 alignment_dir = GetAlignmentVector(nearby_enemies);
        Vector2 cohesion_dir = GetCohesionVector(nearby_enemies);
        Vector2 avoidance_dir = GetAvoidanceVector(asteroid_clearance);

        Vector2 final_desired_dir = Vector2.zero;
        // 3. Combine them (Weighted)
        // 1.0f weight for chasing, 1.5f for separation (don't hit friends!)
        if (avoidance_dir == Vector2.zero) 
        {
            final_desired_dir = (chase_dir * 1.2f) + (separation_dir * 3.0f) + (alignment_dir * 1.5f) + (cohesion_dir * 1.5f);
            final_desired_dir.Normalize();
            turn_speed = default_turn_speed;

        }
        else 
        {
            final_desired_dir = avoidance_dir * 3.0f;
            turn_speed = default_turn_speed * 2.0f;
        }
       

        

        // 4. Rotate the ship toward the FINAL desired direction
        float targetAngle = Mathf.Atan2(final_desired_dir.y, final_desired_dir.x) * Mathf.Rad2Deg - 90f;
        Quaternion target_rot = Quaternion.Euler(0, 0, targetAngle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target_rot, turn_speed * Time.deltaTime);
        
        // 5. Move strictly FORWARD relative to the new rotation
        rb.velocity = transform.up * move_speed;
    }

    void LookAtTarget()
    {
        
        Vector2 direction = rb.velocity;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    Vector2 GetSeperationVector(Collider2D[] enemies)
    {
        //Seperation
        Vector2 total_dir = Vector2.zero;
        int count = 0;
        foreach(var enemy  in enemies) 
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

    Vector2 GetAlignmentVector(Collider2D[] enemies) 
    {
        Vector2 total_dir = Vector2.zero;
        int count = 0;
        foreach(var enemy in enemies) 
        {
            if (enemy.gameObject == gameObject) continue;
            Vector2 velocity = enemy.attachedRigidbody.velocity;
            total_dir += velocity.normalized;
            count++;
        }
        if(count > 0) 
        {
            return ((total_dir/count).normalized);
        }
        return Vector2.zero;
    }

    Vector2 GetCohesionVector(Collider2D[] enemies) 
    {
        Vector2 total_pos = Vector2.zero;
        int count = 0;
        foreach(var enemy in enemies) 
        {
            total_pos += (Vector2)enemy.transform.position;
            count++;
        }
        if(count > 0) 
        {
            Vector2 mean_pos = total_pos / count;
            Vector2 dir = (mean_pos - (Vector2)transform.position).normalized;
            return dir;
        }
        return Vector2.zero;

    }

    Vector2 GetAvoidanceVector(float clearance)
    {
        Vector2 clear_vector = Vector2.zero;
        float angle_step = 1.6f;
        int steps = Mathf.RoundToInt(90.0f / angle_step);
        for(int i = 0; i <= steps; i++) 
        {
            float x1 = clearance * Mathf.Cos(Mathf.Deg2Rad * ((i * angle_step) + transform.eulerAngles.z + 90.0f));
            float y1 = clearance * Mathf.Sin(Mathf.Deg2Rad * ((i * angle_step) + transform.eulerAngles.z + 90.0f));
            float x2 = clearance * Mathf.Cos(Mathf.Deg2Rad * ((-i * angle_step) + transform.eulerAngles.z + 90.0f));
            float y2 = clearance * Mathf.Sin(Mathf.Deg2Rad * ((-i * angle_step) + transform.eulerAngles.z + 90.0f));

            Vector2 positive_angle = new Vector2(x1, y1);
            Vector2 negative_angle = new Vector2(x2, y2);
            RaycastHit2D positive_hit = Physics2D.Raycast(transform.position, positive_angle, clearance, LayerMask.GetMask("Asteroid","Player"));
            RaycastHit2D negative_hit = Physics2D.Raycast(transform.position, negative_angle, clearance, LayerMask.GetMask("Asteroid", "Player"));

            if(!positive_hit) 
            {
                clear_vector = positive_angle;
                //Debug.DrawLine(transform.position, (Vector2)transform.position + (positive_angle * clearance), Color.green, 0.2f);
                if(i == 0) 
                {
                    clear_vector = Vector2.zero;
                }
                break;
            }
            else if (!negative_hit) 
            {
                clear_vector = negative_angle;
                //Debug.DrawLine(transform.position, (Vector2)transform.position + (negative_angle * clearance), Color.green, 0.2f);
                break;
            }
            else 
            {
                clear_vector = positive_angle;
                //Debug.DrawLine(transform.position, (Vector2)transform.position + (positive_angle * clearance), Color.red, 0.2f);
                //Debug.DrawLine(transform.position, (Vector2)transform.position + (negative_angle * clearance), Color.red, 0.2f);  
            }
            
        }
        return clear_vector;
    }
}

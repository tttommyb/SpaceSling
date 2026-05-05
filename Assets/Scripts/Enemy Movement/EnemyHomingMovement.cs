using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHomingMovement : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] GameObject target;

    [Header("Movement")]
    [SerializeField] float move_speed = 15.0f;
    [SerializeField] float default_turn_speed = 1.0f;

    float turn_speed = 1.0f;

    Boid boid;

    Rigidbody2D rb;

    [SerializeField] private LayerMask target_layers;
    [SerializeField] private LayerMask obstructor_layers;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Player");
        rb = GetComponent<Rigidbody2D>();
        turn_speed = default_turn_speed;
        boid = GetComponent<Boid>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 boid_dir = Vector2.zero;
        if (boid.enabled)
        {
            boid_dir = boid.getDesiredDirection(target);
        }

        float targetAngle = Mathf.Atan2(boid_dir.y, boid_dir.x) * Mathf.Rad2Deg - 90f;
        Quaternion target_rot = Quaternion.Euler(0, 0, targetAngle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target_rot, turn_speed * Time.deltaTime);
        rb.velocity = transform.up * move_speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("HIT!");
        if (((1 << other.gameObject.layer) & obstructor_layers) != 0)
        {
            gameObject.GetComponent<Health>().RemoveLife();
            if (((1 << other.gameObject.layer) & target_layers) != 0)
            {
                other.gameObject.GetComponent<Health>().RemoveLife();
            }
        }
    }

}

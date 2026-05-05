using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyTrapperMovement : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] GameObject target;

    [Header("Movement")]
    [SerializeField] float move_speed = 15.0f;
    [SerializeField] float default_turn_speed = 1.0f;

    private static readonly int color_id = Shader.PropertyToID("_Color");


    float turn_speed = 1.0f;

    Rigidbody2D rb;
    LineRenderer lr;

    Boid boid;

    GameObject player;

    GameObject trapped_asteroid = null;
    GameObject connected_asteroid = null;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        target = player;
        rb = GetComponent<Rigidbody2D>();
        boid = GetComponent<Boid>();
        turn_speed = default_turn_speed;
        lr = GetComponent<LineRenderer>();
    }

    void FixedUpdate()
    {
        Vector2 boid_dir = Vector2.zero;
        if (boid.enabled)
        {
            boid_dir = boid.getDesiredDirection(target);
        }

        if((transform.position - target.transform.position).magnitude < 20.0f && connected_asteroid == null) 
        {
            LocateAsteroids();
            if(connected_asteroid != null) 
            {
                lr.SetPosition(0, trapped_asteroid.transform.position);
                lr.SetPosition(1, connected_asteroid.transform.position);
            }
           
        }


        // 4. Rotate the ship toward the FINAL desired direction
        float targetAngle = Mathf.Atan2(boid_dir.y, boid_dir.x) * Mathf.Rad2Deg - 90f;
        Quaternion target_rot = Quaternion.Euler(0, 0, targetAngle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target_rot, turn_speed * Time.deltaTime);

        // 5. Move strictly FORWARD relative to the new rotation
        rb.velocity = transform.up * move_speed;
    }

    void LocateAsteroids() 
    {
        Color color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
        Collider2D asteroid = Physics2D.OverlapCircle((Vector2)player.transform.position + (player.GetComponent<Rigidbody2D>().velocity * 2.0f), 20.0f, LayerMask.GetMask("Asteroid"));
        if (asteroid != null) 
        {
            Collider2D second_asteroid = Physics2D.OverlapCircle(asteroid.transform.position, 10.0f, LayerMask.GetMask("Asteroid"));
            if(second_asteroid != null && second_asteroid != asteroid) 
            {
                asteroid.GetComponent<LineRenderer>().material.SetColor(color_id, color);
                second_asteroid.GetComponent<LineRenderer>().material.SetColor(color_id, color);
                trapped_asteroid = asteroid.gameObject;
                connected_asteroid = second_asteroid.gameObject;
            }
           
        }
      
    }
}

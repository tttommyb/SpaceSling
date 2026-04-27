using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Swinging : MonoBehaviour
{
    Rigidbody2D rb;
    DistanceJoint2D dj;
    LineRenderer lr;
    GameObject attached_asteroid = null;
    GameObject previous_asteroid = null;
    Vector2 asteroid_dir;
    float asteroid_dist;
    float grapple_length;
    float initial_speed; //The velocity when the player first connects to an asteroid
    float current_speed;
    [SerializeField] float asteroid_break_speed = 10;
    [SerializeField] float max_speed = 25;
    [SerializeField] GameObject asteroid_prefab;
    int[] current_quad = new int[]{ 0, 0 };
    int[] previous_quad;
    bool changed_quad = false;
    int[,] quads_gen = new int[ 17, 17];
    int swing_side = 1; //-1 = left 1 = right determines which side of the velocity to swing from
    float target_ortho_size = 10.0f;
    public Vector3 direction = Vector3.zero;

    int player_layer = 0;
    int asteroid_layer = 7;
    // Start is called before the first frame update
    void Start()
    {
        player_layer = LayerMask.NameToLayer("Player");
        asteroid_layer = LayerMask.NameToLayer("Asteroid");

        for(int x = 0; x < 17; x++)
        {
            for (int y = 0; y < 17; y++)
            {
                quads_gen[x, y] = 0;
            }
                
        }

       GenAsteroids(current_quad[0], current_quad[1]);

        rb = transform.GetComponent<Rigidbody2D>();
        dj = transform.GetComponent<DistanceJoint2D>();
        lr = transform.GetComponent<LineRenderer>();
        rb.AddForce(new Vector2(0, 10), ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        LookAtCursor();

        bool should_pass_through = current_speed > asteroid_break_speed;
        Physics2D.IgnoreLayerCollision(player_layer, asteroid_layer, should_pass_through);
    

   

        Camera.main.transform.position = new Vector3(transform.position.x , transform.position.y, -10);

        target_ortho_size = 10.0f + (Mathf.Pow(current_speed,0.8f));

        

        Camera.main.orthographicSize = Mathf.SmoothStep(Camera.main.orthographicSize, target_ortho_size, Camera.main.orthographicSize/target_ortho_size + Time.deltaTime * 0.1f);
        if (attached_asteroid != null)
        {

            asteroid_dir = Vector3.Normalize(attached_asteroid.transform.position - transform.position);
            asteroid_dist = Vector3.Magnitude(attached_asteroid.transform.position - transform.position);

            if (asteroid_dist < 1)
            {
                dj.enabled = false;
                dj.connectedBody = null;
                rb.AddForce(-asteroid_dir * (5 - asteroid_dist) * 5);
            }
            else
            {
                dj.enabled = true;
                dj.connectedBody = attached_asteroid.GetComponent<Rigidbody2D>();
            }


        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            attached_asteroid = FindTargetAsteroid();
            initial_speed = Vector3.Magnitude(rb.velocity);
            lr.enabled = true;
            lr.SetPosition(1, attached_asteroid.transform.position);

        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            BreakConnection();
        }

        current_speed = Vector3.Magnitude(rb.velocity);
        if (Input.GetKey(KeyCode.Mouse0) && current_speed < initial_speed * 1.25 && current_speed < max_speed)
        {
            rb.AddForce(Vector3.Normalize(rb.velocity) * 1.1f);


        }

        current_quad = new int[]{Mathf.RoundToInt(transform.position.x / 100), Mathf.RoundToInt(transform.position.y / 100)};

        if(current_quad != previous_quad)
        {
            GenAsteroids(current_quad[0] - 1, current_quad[1]);
            GenAsteroids(current_quad[0] + 1, current_quad[1]);
            GenAsteroids(current_quad[0], current_quad[1] - 1);
            GenAsteroids(current_quad[0], current_quad[1] + 1);
        }

        previous_quad = current_quad;

        lr.SetPosition(0, transform.position);

        //Asteroid avoidance !!!!!IMPORTANT!!!!!!
        Collider2D[] asteroids = Physics2D.OverlapCircleAll(transform.position, 3.0f, LayerMask.GetMask("Asteroid"));
        direction = Vector3.Normalize(rb.velocity);
        foreach (Collider2D collider in asteroids)
        {
            
            GameObject asteroid = collider.gameObject;
            Vector3 asteroid_dir = Vector3.Normalize(transform.position - asteroid.gameObject.transform.position);
            if (Vector3.Dot(direction, asteroid_dir) > 0)
            {
                //target_ortho_size /= 2.0f;
                continue;
            }
        }

        //Debug.Log("x:" + current_quad[0] + "y:" + current_quad[1]);
    }
    void LookAtCursor()
    {
        Vector3 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 playerPos = transform.position;

        Quaternion rot = Quaternion.LookRotation(playerPos - cursorPos, Vector3.forward);
        transform.rotation = rot;
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z);
    }

    GameObject FindTargetAsteroid()
    {
        Vector3 direction = Vector3.Normalize(rb.velocity);

        Collider2D[] asteroids = Physics2D.OverlapCircleAll(transform.position, 50.0f, LayerMask.GetMask("Asteroid"));
        float smallest_dot = Mathf.Infinity;
        GameObject targeted_asteroid = null;
        foreach (Collider2D asteroid in asteroids)
        {
            if (asteroid.gameObject == previous_asteroid)
            {
                continue;
            }

            if(Vector3.Magnitude(transform.position - asteroid.gameObject.transform.position) < 5)
            {
                continue;
            }

            Vector3 asteroid_dir = Vector3.Normalize(transform.position - asteroid.gameObject.transform.position);
          
            //Check if asteroid is behind if so then ignore
            if (Vector3.Dot(direction, asteroid_dir) > 0)
            {
                continue;
            }
            //Cross Product to check which side to swing to
            if(direction.x  * asteroid_dir.y - direction.y * asteroid_dir.x < 0 && swing_side == 1)
            {
                continue;
            }
            else if (direction.x * asteroid_dir.y - direction.y * asteroid_dir.x > 0 && swing_side == -1)
            {
                continue;
            }
            float dot = Vector3.Dot(direction, new Vector2(asteroid_dir.y, -asteroid_dir.x));
            if (dot < smallest_dot)
            {
                smallest_dot = dot;
                targeted_asteroid = asteroid.gameObject;
            }
        }
        return targeted_asteroid;
    }

    void BreakConnection()
    {
        previous_asteroid = attached_asteroid;
        attached_asteroid = null;
        dj.connectedBody = null;
        dj.enabled = false;
        lr.enabled = false;
        swing_side *= -1;
    }

    void GenAsteroids(int quad_x, int quad_y)
    {


        if(quads_gen[quad_x + 8, quad_y + 8] == 1) { return; }
        for (int x = 0; x < 50; x++)
        {
                Vector2 pos = new Vector2(
                        Random.Range((quad_x * 100) - 50, (quad_x * 100) + 50),
                     Random.Range((quad_y * 100) - 50, (quad_y * 100) + 50)
               );
                Instantiate(asteroid_prefab, pos, Quaternion.identity);
        }
        quads_gen[quad_x + 8, quad_y + 8] = 1;
    }


}

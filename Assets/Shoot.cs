using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    [SerializeField] GameObject bullet;
    GameObject bullet_instance;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) 
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0; 
            Vector2 dir = (mousePos - transform.position).normalized;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            bullet_instance = Instantiate(bullet, transform.position, rotation);

            Rigidbody2D bullet_rb = bullet_instance.GetComponent<Rigidbody2D>();

            // 3. GET THE PLAYER'S VELOCITY
            // Assuming this script is on the Player who has a Rigidbody2D
            Vector2 player_velocity = transform.parent.GetComponent<Rigidbody2D>().velocity;

            // 4. APPLY INHERITED VELOCITY + BULLET SPEED
            // We set the velocity directly to ensure it 'starts' at the player's speed,
            // then add the muzzle flash speed in the direction we are pointing.
            bullet_rb.velocity = player_velocity + (dir * 10.0f);
        }
    }
}

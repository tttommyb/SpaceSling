using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    [SerializeField] GameObject bullet;
    [SerializeField] float speed = 50.0f;
    Projectile projectile;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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

            projectile = Instantiate(bullet, transform.position, rotation).GetComponent<Projectile>();
            projectile.Initialise(rb.velocity);
        }
    }
}

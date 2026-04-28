using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    [Tooltip("Which layers should this bullet collide with?")]
    [SerializeField] private LayerMask target_layers;

    [SerializeField] private float speed = 50.0f;
    [Tooltip("How long will it exist for?")]
    [SerializeField] float life_time = 5.0f;
    float current_time = 0.0f;
    SpriteRenderer sr;
    private static readonly int opacity_id = Shader.PropertyToID("_Opacity");


    // Start is called before the first frame update
    void Start()
    {
        current_time = life_time;
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        current_time -= Time.deltaTime;
        sr.material.SetFloat(opacity_id, current_time / life_time);
        if (current_time <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
            Debug.Log("HIT!");
        if (((1 << other.gameObject.layer) & target_layers) != 0)
        {

            Destroy(this.gameObject);
        }
    }

    public void Initialise(Vector2 starting_velocity) 
    {
        GetComponent<Rigidbody2D>().velocity = starting_velocity + (Vector2.up * speed);
    }
}

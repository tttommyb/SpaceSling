using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{


    [SerializeField] private float speed = 50.0f;
    [Tooltip("How long will it exist for?")]
    [SerializeField] float life_time = 5.0f;
    float current_time = 0.0f;
    SpriteRenderer sr;
    private static readonly int opacity_id = Shader.PropertyToID("_Opacity");
    private static readonly int color_id = Shader.PropertyToID("_Color");
    private LayerMask target_layers;
    private LayerMask obstructor_layers;


    // Start is called before the first frame update
    void Start()
    {
        current_time = life_time;
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
        if (((1 << other.gameObject.layer) & obstructor_layers) != 0)
        {
            Destroy(this.gameObject);
            if (((1 << other.gameObject.layer) & target_layers) != 0)
            {
                other.gameObject.GetComponent<Health>().RemoveLife();
            }
        }
    }

    public void Initialise(Vector2 starting_velocity, LayerMask targets, LayerMask obstructors, Color color) 
    {
        sr = GetComponent<SpriteRenderer>();
        GetComponent<Rigidbody2D>().velocity = starting_velocity + (GetComponent<Rigidbody2D>().GetRelativeVector(Vector2.up) * speed);
        target_layers = targets;
        obstructor_layers = obstructors;
        sr.material.SetColor(color_id, color);
        
    }
}

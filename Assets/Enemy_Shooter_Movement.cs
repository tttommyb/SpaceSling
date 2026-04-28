using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Shooter_Movement : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private GameObject target;
    Vector2 target_position;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        target_position = target.transform.position;
        Vector2 dir = (target_position - (Vector2)transform.position).normalized;
        rb.velocity = (dir * 10.0f);
        LookAtTarget();
    }


    void LookAtTarget()
    {
        
        Vector2 direction = (target_position - (Vector2)transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

}

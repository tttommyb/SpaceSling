using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.Intrinsics;
using UnityEngine;



public class Break : MonoBehaviour
{

    LineRenderer lr;

    List<Vector3> debugPointsA = new List<Vector3>();
    List<Vector3> debugPointsB = new List<Vector3>();
    List<Vector3> debugPointsC = new List<Vector3>();
    Vector3 hit_pos = Vector3.zero;
    Vector3 intercept_pos = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb == null) return;


        Vector2 point = other.transform.position;
        Vector2 dir = rb.velocity.normalized;
        RaycastHit2D[] hits = Physics2D.RaycastAll(point, dir, 5.0f, LayerMask.GetMask("Asteroid"));
        Debug.DrawRay(point, dir * 5f, Color.red, 10.0f);
        foreach (RaycastHit2D hit in hits )
        {
            debugPointsC.Add(hit.point);
        }
       
        hit_pos = point;
        GetComponent<EdgeCollider2D>().enabled = false;

        return;
    }

    private void OnDrawGizmos()
    {
        //if(intercept_pos == Vector3.zero) return;

        // Draw Side A points in Green
        Gizmos.color = Color.green;
        foreach (var p in debugPointsA)
        {
            // Convert local back to world for drawing
            Gizmos.DrawSphere(transform.TransformPoint(p), 0.05f);
        }

        // Draw Side B points in Red
        Gizmos.color = Color.red;
        foreach (var p in debugPointsB)
        {
            Gizmos.DrawSphere(transform.TransformPoint(p), 0.05f);
        }

        Gizmos.color = Color.yellow;
        foreach (var p in debugPointsC)
        {
            Gizmos.DrawSphere(p, 0.05f);
        }



        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(hit_pos, 0.05f);
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(intercept_pos, 0.05f);
    }


    bool IsPointOnSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        float dist = Vector2.Distance(a, b);
        return Vector2.Distance(a, p) + Vector2.Distance(p, b) <= dist + 0.01f;
    }
}




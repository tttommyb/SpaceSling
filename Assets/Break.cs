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

    Color unique_col;

   Vector3 hit_pos = Vector3.zero;
    Vector3 intercept_pos = Vector3.zero;


    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        unique_col = new Color(Random.Range(0, 255) / 255.0f, Random.Range(0, 255) / 255.0f, Random.Range(0, 255) / 255.0f);

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
        if (dir == Vector2.zero) return;

        debugPointsC.Clear();

        // 1. ENTRY POINT: Start a unit back, shoot forward
        Vector2 entry_origin = point - (dir * 5.0f);
        Vector2 hit = GetSpecificHit(entry_origin, dir, 15.0f, GetComponent<EdgeCollider2D>());
        debugPointsC.Add(hit);
        Debug.Log("ENTRY: " + debugPointsC[0]);

        // 2. EXIT POINT: Start 5 units ahead, shoot backward
        Vector2 exit_origin = point + (dir * 10.0f);
        Debug.DrawRay(exit_origin, -dir * 15.0f, unique_col, 200.0f);
        hit = GetSpecificHit(exit_origin, -dir, 15.0f, GetComponent<EdgeCollider2D>());
        debugPointsC.Add(hit);
        Debug.Log("EXIT:" + debugPointsC[1]);

       


        hit_pos = entry_origin;
        intercept_pos = exit_origin;

        GetComponent<EdgeCollider2D>().enabled = false;
    }

    private Vector2 GetSpecificHit(Vector2 origin, Vector2 direction, float distance, Collider2D target)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, distance);

        RaycastHit2D closest_hit = new RaycastHit2D();
        float min_distance = float.MaxValue;
        bool found = false;

        foreach (var hit in hits)
        {
            // 1. Is it the right asteroid?
            // 2. Is it closer than any other hit we've found on this asteroid so far?
            if (hit.collider == target && hit.distance < min_distance)
            {
                min_distance = hit.distance;
                closest_hit = hit;
                found = true;
            }
        }

        return found ? closest_hit.point : Vector2.zero;
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


        Gizmos.color = unique_col;
        foreach (var p in debugPointsC)
        {
            Gizmos.DrawSphere(p, 0.05f);
        }



        if (hit_pos != Vector3.zero && intercept_pos != Vector3.zero) 
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(hit_pos, 0.05f);
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(intercept_pos, 0.05f);
        }
   
    }


    bool IsPointOnSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        float dist = Vector2.Distance(a, b);
        return Vector2.Distance(a, p) + Vector2.Distance(p, b) <= dist + 0.01f;
    }
}




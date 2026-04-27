using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEditor;
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

    [SerializeField] GameObject graze_particle_system;



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
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;

        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb == null) return;


        

        Vector2 point = other.transform.position;
        Vector2 dir = rb.velocity.normalized;
        if (dir == Vector2.zero) return;

        debugPointsC.Clear();

        // 1. ENTRY POINT: Start a unit back, shoot forward
        Vector2 entry_origin = point - (dir * 5.0f);
        Vector2 entry_hit = GetSpecificHit(entry_origin, dir, 15.0f, GetComponent<EdgeCollider2D>());
        debugPointsC.Add(entry_hit);

        // 2. EXIT POINT: Start 5 units ahead, shoot backward
        Vector2 exit_origin = point + (dir * 10.0f);
        Debug.DrawRay(exit_origin, -dir * 15.0f, unique_col, 200.0f);
        Vector2 exit_hit = GetSpecificHit(exit_origin, -dir, 15.0f, GetComponent<EdgeCollider2D>());
        debugPointsC.Add(exit_hit);

        Vector3[] points = new Vector3[lr.positionCount];
        lr.GetPositions(points);

        List<Vector3> asteroid_A_points = new List<Vector3>();
        List<Vector3> asteroid_B_points = new List<Vector3>();

        Vector2 local_entry = transform.InverseTransformPoint(entry_hit);
        Vector2 local_exit = transform.InverseTransformPoint(exit_hit);

        for (int i = 0; i < lr.positionCount; ++i) 
        {
            
            Vector2 current = points[i];
            Vector2 next = points[(i + 1) % points.Length]; // The next point in the loop

            // Side check for the CURRENT point
            float side = (local_entry.x - local_exit.x) * (current.y - local_exit.y) -
                         (local_entry.y - local_exit.y) * (current.x - local_exit.x);

            if (side >= 0) asteroid_A_points.Add(current);
            else asteroid_B_points.Add(current);

            // CHECK FOR CROSSING: Did this specific segment (current to next) contain a hit?
            if (IsPointOnSegment(local_entry, current, next))
            {
                asteroid_A_points.Add(local_entry);
                asteroid_B_points.Add(local_entry);
            }

            if (IsPointOnSegment(local_exit, current, next))
            {
                asteroid_A_points.Add(local_exit);
                asteroid_B_points.Add(local_exit);
            }

        }

        hit_pos = entry_origin;
        intercept_pos = exit_origin;

        lr.positionCount = asteroid_A_points.Count;
        lr.SetPositions(asteroid_A_points.ToArray());
    
        GetComponent<EdgeCollider2D>().enabled = false;

        lr.positionCount = asteroid_A_points.Count;
        lr.SetPositions(asteroid_A_points.ToArray());

        // 2. Create the second half (Side B)
        if (asteroid_B_points.Count > 2)
        {
            // Instantiate at the SAME position and rotation as the original
            GameObject other_side = Instantiate(this.gameObject, transform.position, transform.rotation);

            // IMPORTANT: If 'this.gameObject' had this script, the new one does too. 
            // Destroy the script on the new one so it doesn't try to 'break' again immediately.
            //Destroy(other_side.GetComponent<Break>());

            LineRenderer other_lr = other_side.GetComponent<LineRenderer>();
            other_lr.positionCount = asteroid_B_points.Count;
            other_lr.SetPositions(asteroid_B_points.ToArray());
            //other_lr.material.color = Color.blue;

            Rigidbody2D b_rb = other_side.GetComponent<Rigidbody2D>();
            Rigidbody2D a_rb = GetComponent<Rigidbody2D>();

            Vector2 player_norm = new Vector2(-dir.y, dir.x);
            if (a_rb != null)
            {
                a_rb.bodyType = RigidbodyType2D.Dynamic;
                // Push it away from the center of the cut
                a_rb.AddForce(player_norm * -10f, ForceMode2D.Impulse);
            }
            if (b_rb != null)
            {
                b_rb.bodyType = RigidbodyType2D.Dynamic;
                // Push it away from the center of the cut
                b_rb.AddForce(player_norm * 10f, ForceMode2D.Impulse);
                rb.AddRelativeForce(Vector2.up * -10.0f, ForceMode2D.Impulse);
            }

            // Optional: Update the EdgeCollider2D points for both so they can be hit again
        }
        else { Instantiate(graze_particle_system, entry_hit, Quaternion.identity); }

        // Disable the original collider so we don't trigger multiple times in one frame
        GetComponent<EdgeCollider2D>().enabled = false;
        Destroy(this); // Remove this script from the original part

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




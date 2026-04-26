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
        if (!other.CompareTag("Player")) return;
        
         ColliderDistance2D hit = other.Distance(GetComponent<Collider2D>());
        if (!hit.isValid) return;
            
        Vector2 hit_point = hit.pointA;
        GameObject other_game_object = other.gameObject;
        Rigidbody2D rb = other_game_object.GetComponent<Rigidbody2D>();
        Vector2 direction = Vector3.zero;
        float player_c = 0; //Y intercept of the players line equation
        float player_m = 0; //Gradient of the players line equation
        if (rb != null)
        {
            direction = rb.velocity.normalized;
            player_m = direction.y / direction.x;
            player_c = hit_point.y - (hit_point.x * player_m);

            for (int i = 0; i < lr.positionCount; i++)
            {
                Vector2 edge = lr.GetPosition(0) - lr.GetPosition(lr.positionCount - 1);
                Vector3 point1 = lr.GetPosition(0);
                Vector3 point2 = lr.GetPosition(lr.positionCount - 1);
                if (i != 0)
                {
                    edge = lr.GetPosition(i) - lr.GetPosition(i - 1);
                    point1 = lr.GetPosition(i);
                    point2 = lr.GetPosition(i - 1);
                }
                point1 += transform.position;
                point2 += transform.position;
                Vector2 edge_direction = edge.normalized;
                float edge_m = edge_direction.y / edge_direction.x; //Gradient of edge line
                float edge_c = (point1.y) - (edge_m * point1.x); //Y intercept of the edge line
                float intercept_x = (edge_c - player_c) / (player_m - edge_m);
                if (intercept_x >= Mathf.Min(point1.x, point2.x) && intercept_x <= Mathf.Max(point1.x, point2.x))
                {
                    float intercept_y = (intercept_x * player_m) + player_c;
                    if (intercept_y >= Mathf.Min(point1.y, point2.y) && intercept_y <= Mathf.Max(point1.y, point2.y))
                    {
                        Vector3 intercept = new Vector3(intercept_x, intercept_y);
                        Color color = new Color(Random.Range(0.0f, 255.0f) / 255, Random.Range(0.0f, 255.0f) / 255, Random.Range(0.0f, 255.0f) / 255);
                        Debug.DrawLine(hit_point, new Vector2(intercept_x, intercept_y), color, 5.0f);

                        Vector3[] positions = new Vector3[lr.positionCount];
                        lr.GetPositions(positions);
                        List<Vector3> asteroid_A_points = new List<Vector3>();
                        List<Vector3> asteroid_B_points = new List<Vector3>();

                        Vector2 local_hit = transform.InverseTransformPoint(hit_point);
                        Vector2 local_intercept = transform.InverseTransformPoint(intercept);

                        asteroid_A_points.Add(transform.TransformPoint(local_hit)); // 1. Start at the cut entry

                        for (int j = 0; j < positions.Length; j++)
                        {
                            float xp = (local_hit.x - local_intercept.x) * (positions[j].y - local_intercept.y) -
                                      (local_hit.y - local_intercept.y) * (positions[j].x - local_intercept.x);

                            if (xp < 0)
                            {
                                // 2. Only add vertices if they are on the 'A' side of the blade
                                asteroid_A_points.Add(transform.TransformPoint(positions[j]));
                            }
                        }

                        asteroid_A_points.Add(transform.TransformPoint(local_intercept));


                        asteroid_B_points.Add(transform.TransformPoint(local_hit));

                        for (int j = positions.Length - 1; j >= 0; j--)
                        {
                            float xp = (local_hit.x - local_intercept.x) * (positions[j].y - local_intercept.y) -
                                      (local_hit.y - local_intercept.y) * (positions[j].x - local_intercept.x);

                            if (xp >= 0)
                            {
                                // 2. Only add vertices if they are on the 'B' side of the blade
                                asteroid_B_points.Add(transform.TransformPoint(positions[j]));
                            }
                        }
                        asteroid_B_points.Add(transform.TransformPoint(local_intercept));



                        lr.useWorldSpace = true;
                        GameObject asteroid_new = Instantiate(this.gameObject, transform.position, Quaternion.identity);

                        
                        lr.positionCount = asteroid_A_points.Count;
                        lr.SetPositions(asteroid_A_points.ToArray());

                        asteroid_new.GetComponent<LineRenderer>().positionCount = asteroid_B_points.Count;
                        asteroid_new.GetComponent<LineRenderer>().SetPositions(asteroid_B_points.ToArray());
                        asteroid_new.GetComponent<EdgeCollider2D>().enabled = false;
                        GetComponent<EdgeCollider2D>().enabled = false;

                        // Clear the old ones first
                        debugPointsA.Clear();
                        debugPointsB.Clear();

                        // After your sorting loop finishes:
                        debugPointsA.AddRange(asteroid_A_points);
                        debugPointsB.AddRange(asteroid_B_points);

                        hit_pos = local_hit;
                        intercept_pos = local_intercept;

                        lr.material.color = Color.cyan;

                        return;







                    }



                }

            }

        }

    }

    private void OnDrawGizmos()
    {
        if(intercept_pos == Vector3.zero) return;

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

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.TransformPoint(hit_pos), 0.05f);
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(transform.TransformPoint(intercept_pos), 0.05f);
        Gizmos.color = Color.black;
    }


    bool IsPointOnSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        float dist = Vector2.Distance(a, b);
        return Vector2.Distance(a, p) + Vector2.Distance(p, b) <= dist + 0.01f;
    }
}




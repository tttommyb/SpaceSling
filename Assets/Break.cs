using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.Intrinsics;
using UnityEngine;



public class Break : MonoBehaviour
{

    LineRenderer lr;
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
        if (other.CompareTag("Player"))
        {
            ColliderDistance2D hit = other.Distance(GetComponent<Collider2D>());
            if (hit.isValid)
            {
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
                                Debug.DrawRay(hit_point, direction, Color.blue);

                                Vector3[] positions = new Vector3[lr.positionCount];
                                lr.GetPositions(positions);
                                List<Vector3> asteroid_A_points = new List<Vector3>();
                                List<Vector3> asteroid_B_points = new List<Vector3>();

                                Debug.LogWarning("Intercept: " + intercept);
                                Debug.LogWarning("Point: " + hit_point);

                                hit_point -= new Vector2(transform.position.x, transform.position.y);
                                intercept -= transform.position;

                                asteroid_A_points.Add(hit_point);
                                asteroid_B_points.Add(hit_point);


                                for (int j = 0; j < lr.positionCount; j++)
                                {

                                    Vector3 position = positions[j];
                                    float xp = (hit_point.x - intercept.x) * (position.y - intercept.y) - (hit_point.y - intercept.y) * (position.x - intercept.x); //CHeck which side of breakline position lies on
                                    if (xp < 0.0f)
                                    {
                                        asteroid_A_points.Add(position);
                                    }
                                    else
                                    {
                                        asteroid_B_points.Add(position);
                                    }

                                    if (j == i)
                                    {
                                        asteroid_A_points.Add(intercept);
                                        asteroid_B_points.Add(intercept);

                                    }

                                    int index_1 = j;
                                    int index_2 = j - 1;
                                    if (index_2 < 0)
                                    {
                                        index_2 = lr.positionCount - 1;
                                    }

                                    Vector2 line_point_A = positions[index_1];
                                    Vector2 line_point_B = positions[index_2];

                                    Vector2 AB_vector = line_point_B - line_point_A;
                                    Vector2 AP_vector = hit_point - line_point_A;

                                    xp = (hit_point.x - line_point_A.x) * (line_point_B.y - line_point_A.y) - (hit_point.y - line_point_A.y) * (line_point_B.x - line_point_A.x); //CHeck which side of breakline position lies on
                                    if (xp == 0) 
                                    {
                                        if(0 <= Vector2.Dot(AP_vector, AB_vector) && Vector2.Dot(AP_vector, AB_vector) <= Vector2.Dot(AB_vector, AB_vector)) 
                                        {
                                            asteroid_A_points.Add(hit_point);
                                            asteroid_B_points.Add(hit_point);
                                        }
                                    }


                                }


                                lr.SetPositions(asteroid_A_points.ToArray());
                                GetComponent<EdgeCollider2D>().enabled = false;

                                return;

                            }



                        }

                    }
                }




            }

        }

    }
}

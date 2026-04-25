using System.Collections;
using System.Collections.Generic;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Vector2 point = collision.contacts[0].point;
        Debug.Log(point);
        GameObject other_game_object = collision.collider.gameObject;
        Rigidbody2D rb = other_game_object.GetComponent<Rigidbody2D>();
        Vector2 direction = Vector3.zero;
        float player_c = 0; //Y intercept of the players line equation
        float player_m = 0; //Gradient of the players line equation
        if (rb != null)
        {
            direction = rb.velocity.normalized;
            player_m = direction.y / direction.x;
            player_c = point.y - (point.x * player_m);

            for (int i = 0; i < lr.positionCount; i++)
            {
                Vector2 edge = lr.GetPosition(0) - lr.GetPosition(lr.positionCount - 1);
                Vector3 point1 = lr.GetPosition(0);
                Vector3 point2 = lr.GetPosition(lr.positionCount - 1);
                if (i != 0)
                {
                    edge = lr.GetPosition(i) - lr.GetPosition(i - 1);
                    point1 = lr.GetPosition(i);
                    point2 = lr.GetPosition(i -1);
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
                        Color color = new Color(Random.Range(0.0f, 255.0f) / 255, Random.Range(0.0f, 255.0f) / 255, Random.Range(0.0f, 255.0f) / 255);
                        Debug.DrawLine(point, new Vector2(intercept_x, intercept_y), color, 5.0f);
                        Debug.DrawRay(point, direction, Color.blue);
                        Debug.Break();

                    }



                }

            }
        }



    }    
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenAsteroid : MonoBehaviour
{
    List<Vector2> asteroid_points2 = new List<Vector2>();
    List<Vector3> asteroid_points3 = new List<Vector3>();
    LineRenderer lr;
    PolygonCollider2D pc;
    EdgeCollider2D ec;

    
    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        ec = GetComponent<EdgeCollider2D>();

        int points = Random.Range(4, 8);
        lr.positionCount = points;

        for(int i = 0; i < points; i++)
        {
            float angle = (i /(float)points) * 360.0f;
            float x = Vector2.up.x;
            float y = Vector2.up.y;
            Vector2 point_pos = new Vector2(x * Mathf.Cos(Mathf.Deg2Rad * angle) - y * Mathf.Sin(Mathf.Deg2Rad * angle),
                                x * Mathf.Sin(Mathf.Deg2Rad * angle) + y * Mathf.Cos(Mathf.Deg2Rad * angle));
            float displacement = Random.Range(0.5f, 1.8f);
            asteroid_points2.Add(point_pos * displacement);
            asteroid_points3.Add(point_pos * displacement);

        }
        asteroid_points2.Add(asteroid_points2[0]);        
        asteroid_points3.Add(asteroid_points3[0]);        

        lr.SetPositions(asteroid_points3.ToArray());
        ec.points = asteroid_points2.ToArray();
    }

    // Update is called once per frame
    void Update()
    {
    }
}

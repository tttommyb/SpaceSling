using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstroidMotion : MonoBehaviour
{
    Vector3 rotation_rate = new Vector3();

    private void Start()
    {
        rotation_rate.z = Random.Range(1.0f, 12.5f);
    }
    void FixedUpdate()
    {
        transform.Rotate(rotation_rate * Time.deltaTime);
    }
}

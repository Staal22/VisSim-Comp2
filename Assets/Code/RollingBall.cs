using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingBall : MonoBehaviour
{
    private int _triangle;
    
    private void Start()
    {
        gameObject.transform.position = new Vector3(0.6f, 20.1f, 0.6f);
    }

    private void Update()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out var hit, 5f))
        {
            if (hit.transform.gameObject.TryGetComponent<TriangleSurface>(out var triangleSurface))
            {
                _triangle = triangleSurface.FindTriangle(hit.point);
                    if (_triangle != -1)
                        Debug.Log("Rolling on triangle " + _triangle);
            }
        }
    }
}

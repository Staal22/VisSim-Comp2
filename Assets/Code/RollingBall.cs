using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingBall : MonoBehaviour
{
    private void Start()
    {
        gameObject.transform.position = new Vector3(0.6f, 20.1f, 0.6f);
    }
}

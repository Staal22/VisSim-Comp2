using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RollingBall : MonoBehaviour
{
    private Rigidbody _rb;
    private int _triangle;
    private Vector3 _oldVelocity;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        gameObject.transform.position = new Vector3(0.6f, 20.1f, 0.6f);
        _oldVelocity = _rb.velocity;
    }

    private void FixedUpdate()
    {
        Vector3 unitNormal = Vector3.zero;
        if (Physics.Raycast(transform.position, Vector3.down, out var hit, 5f))
        {
            if (hit.transform.gameObject.TryGetComponent<TriangleSurface>(out var triangleSurface))
            {
                _triangle = triangleSurface.FindTriangle(hit.point);
                if (_triangle != -1)
                    Debug.Log("Rolling on triangle " + _triangle);
            }

            unitNormal = hit.normal;
            unitNormal = unitNormal.normalized;
        }
        
        // gravity
        var gravity = Physics.gravity * _rb.mass;
        // surface normal
        var surfaceNormal = Vector3.Dot(gravity, unitNormal) * unitNormal;
        // acceleration
        var acceleration = (gravity + surfaceNormal)/_rb.mass;
        // velocity
        var velocity = _oldVelocity + acceleration * Time.fixedDeltaTime;
        _oldVelocity = velocity;
        // position
        var position = transform.position + velocity * Time.fixedDeltaTime;
        // // move
        // transform.position = position;
        
        _rb.AddForce(acceleration, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision other)
    {
        // bounce force
        // _rb.AddForce(other.contacts[0].normal * 10f, ForceMode.Impulse);
    }
}

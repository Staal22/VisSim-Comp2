using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RollingBall : MonoBehaviour
{
    [SerializeField] private TriangleSurface triangleSurface;
    private Rigidbody _rb;
    private int _triangle = 0;
    private int _lastTriangle = 0;
    private float _radius;
    private bool _rolling;
    private Vector3 _oldVelocity;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _radius = gameObject.transform.localScale.x / 2;
    }

    private void Start()
    {
        gameObject.transform.position = new Vector3(_radius, 19.0f + _radius, _radius);
        _oldVelocity = _rb.velocity;
    }

    private void FixedUpdate()
    {
        _lastTriangle = _triangle;
        var unitNormal = Vector3.zero;
        var triangles = triangleSurface.Triangles;
        
        var gravity = Physics.gravity * _rb.mass;
        
        if (Physics.Raycast(transform.position, Vector3.down, out var hit, 20f))
        {
            _rolling = Physics.Raycast(transform.position, Vector3.down, _radius + 0.1f);
            if (hit.transform.gameObject.TryGetComponent<TriangleSurface>(out var surface))
            {
                _triangle = surface.FindTriangle(hit.point);
                if (_triangle != -1)
                    Debug.Log("Rolling on triangle " + _triangle);
            }
            var edge1 = triangles[_triangle][1] - triangles[_triangle][0];
            var edge2 = triangles[_triangle][2] - triangles[_triangle][0];
            unitNormal = Vector3.Cross(edge1, edge2).normalized;
        }
        var surfaceNormal = -Vector3.Dot(gravity, unitNormal) * unitNormal;
        var force = gravity + surfaceNormal;
        Vector3 acceleration;
        // if (_rolling)
            acceleration = force / _rb.mass;
        // else
        //     acceleration = gravity / _rb.mass;
        if (_lastTriangle != _triangle)
        {
            var lastTriangleNormal = Vector3.Cross(triangles[_lastTriangle][1] - triangles[_lastTriangle][0],
                triangles[_lastTriangle][2] - triangles[_lastTriangle][0]).normalized;
            var triangleNormal = Vector3.Cross(triangles[_triangle][1] - triangles[_triangle][0],
                triangles[_triangle][2] - triangles[_triangle][0]).normalized;
            var reflectionNormal = (lastTriangleNormal + triangleNormal).normalized;
            _oldVelocity -= 2 * Vector3.Dot(_oldVelocity, reflectionNormal) * reflectionNormal;
        }
        var velocity = _oldVelocity + acceleration * Time.fixedDeltaTime;
        _oldVelocity = velocity;
        var position = transform.position + velocity * Time.fixedDeltaTime;
        transform.position = position;
    }
}

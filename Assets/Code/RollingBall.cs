using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RollingBall : MonoBehaviour
{
    [SerializeField] private TriangleSurface triangleSurface;
    private const float Mass = 1;
    private int _triangle = 0;
    private int _nextTriangle = 0;
    private float _radius;
    private bool _rolling;
    private float _height;
    private Vector3 _oldVelocity;

    private void Awake()
    {
        _radius = gameObject.transform.localScale.x / 2;
    }

    private void Start()
    {
        gameObject.transform.position = new Vector3(_radius, 19.0f, _radius);
        _oldVelocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        var unitNormal = Vector3.zero;
        var triangles = triangleSurface.Triangles;
        
        var gravity = Physics.gravity * Mass;
        
        _triangle = triangleSurface.FindTriangle(transform.position);
        if (_triangle != -1)
        {
            Debug.Log("Triangle: " + _triangle);
            var edge1 = triangles[_triangle][1] - triangles[_triangle][0];
            var edge2 = triangles[_triangle][2] - triangles[_triangle][0];
            unitNormal = Vector3.Cross(edge1, edge2).normalized;
            
            Vector3 point = triangles[_triangle][0];
            // _height = point.y;
            var p = transform.position - point;
            // project p onto unit normal
            var y = Vector3.Dot(p, unitNormal) * unitNormal;
            _rolling = y.magnitude <= _radius;
            Debug.Log("Rolling: " + _rolling);
            if (_rolling)
            {
                // find plane height from barycentric coordinates
                var barycentricCoordinates = Utilities.Barycentric(
                    triangles[_triangle][0],
                    triangles[_triangle][1],
                    triangles[_triangle][2],
                    transform.position
                );
                _height = barycentricCoordinates.x * triangles[_triangle][0].y +
                          barycentricCoordinates.y * triangles[_triangle][1].y +
                          barycentricCoordinates.z * triangles[_triangle][2].y;
            }
        }
        else
        {
            _rolling = false;
            Debug.Log("No triangle found");
        }
        
        var surfaceNormal = -Vector3.Dot(gravity, unitNormal) * unitNormal;
        var force = gravity + surfaceNormal;
        Vector3 acceleration;
        if (_rolling)
            acceleration = force / Mass;
        else
            acceleration = gravity / Mass;
        // Draw debug line for acceleration
        Debug.DrawRay(transform.position, acceleration, Color.red);
        
        var velocity = _oldVelocity + acceleration * Time.fixedDeltaTime;
        // Draw debug line for velocity
        Debug.DrawRay(transform.position, velocity, Color.green);

        Vector3 nextPosition = transform.position + velocity * Time.fixedDeltaTime;
        _nextTriangle = triangleSurface.FindTriangle(nextPosition);
        if (_nextTriangle != _triangle && _nextTriangle != -1 && _triangle != -1)
        {
            var nextTriangleNormal = Vector3.Cross(triangles[_nextTriangle][1] - triangles[_nextTriangle][0],
                triangles[_nextTriangle][2] - triangles[_nextTriangle][0]).normalized;
            var triangleNormal = Vector3.Cross(triangles[_triangle][1] - triangles[_triangle][0],
                triangles[_triangle][2] - triangles[_triangle][0]).normalized;
            var reflectionNormal = (nextTriangleNormal + triangleNormal).normalized;
            
            var edge1 = triangles[_nextTriangle][1] - triangles[_nextTriangle][0];
            var edge2 = triangles[_nextTriangle][2] - triangles[_nextTriangle][0];
            unitNormal = Vector3.Cross(edge1, edge2).normalized;
            
            Vector3 point = triangles[_nextTriangle][0];
            var p = nextPosition - point;
            Debug.DrawRay(point, p, Color.blue);
            // UnityEditor.EditorApplication.isPaused = true;
            var y = Vector3.Dot(p, unitNormal) * unitNormal;
            var crash = y.magnitude < _radius;
            
            // if(Vector3.Cross(nextTriangleNormal, triangleNormal) > )
            
            if (crash)
                velocity -= 2 * Vector3.Dot(velocity, reflectionNormal) * reflectionNormal;
        }
        _oldVelocity = velocity;
        
        
        var position = transform.position + velocity * Time.fixedDeltaTime;
        transform.position = position;
        // transform.position = _rolling ? new Vector3(position.x, _height + _radius, position.z) : position;
    }
}

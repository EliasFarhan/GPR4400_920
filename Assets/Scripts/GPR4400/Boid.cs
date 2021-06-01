using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    Vector3 position = Vector3.zero;
    
    private Vector3 velocity = Vector3.zero;

    public Vector3 Velocity
    {
        get => velocity;
        set => velocity = value;
    }

    public Vector3 Position
    {
        get => position;
        set => position = value;
    }

    private Transform boidTransform;

    private void Start()
    {
        boidTransform = transform;
        position = boidTransform.position;
    }

    private void Update()
    {
        var position1 = boidTransform.position;
        position1 += velocity * Time.deltaTime;
        boidTransform.position = position1;
        position = position1;


        var angles = boidTransform.eulerAngles;
        angles.z = Vector3.SignedAngle(Vector3.up, velocity.normalized, Vector3.forward);
        boidTransform.eulerAngles = angles;
    }
}

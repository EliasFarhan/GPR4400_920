using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringWithAvoidanceEntity : MonoBehaviour
{
    [SerializeField] private Transform pathParent;
    private Vector3[] path;
    private int targetIndex = 0;
    private Transform entityTransform;
    private Rigidbody2D body;
    CircleCollider2D circleCollider;

    [SerializeField] private float entitySpeed = 3.0f;
    [SerializeField] private float pointThreshold = 0.5f;
    [SerializeField] private float maxSteeringForce = 10.0f;
    [SerializeField] float maxAvoidanceDistance = 5.0f;
    RaycastHit2D[] hits;
    // Start is called before the first frame update
    void Start()
    {
        hits = new RaycastHit2D[5];
        body = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        entityTransform = transform;
        path = new Vector3[pathParent.childCount];
        for (int i = 0; i < pathParent.childCount; i++)
        {
            path[i] = pathParent.GetChild(i).position;
        }
    }

    private void FixedUpdate()
    {
        var entityPosition = entityTransform.position;
        var targetPosition = path[targetIndex];
        var deltaPos = targetPosition - entityPosition;
        if (deltaPos.magnitude < pointThreshold)
        {
            targetIndex++;
            if (targetIndex >= path.Length)
                targetIndex = 0;
        }
        else
        {
            var dir = deltaPos.normalized;
            Vector2 targetVelocity = entitySpeed * dir;

            
            var currentVelocity = body.velocity;
            var deltaVelocity = targetVelocity - currentVelocity;
            var force = body.mass * deltaVelocity / Time.fixedDeltaTime;
            if (force.magnitude > maxSteeringForce)
            {
                force = force.normalized * maxSteeringForce;
            }
            Vector2[] origins = new Vector2[]
            {
                entityPosition,
                entityPosition + new Vector3(dir.y, -dir.x) * circleCollider.radius,
                entityPosition + new Vector3(-dir.y, dir.x) * circleCollider.radius,
            };
            Vector2 avoidanceVelocity = Vector2.zero;
            foreach (var origin in origins)
            {
                int count = Physics2D.RaycastNonAlloc(origin, dir, hits, maxAvoidanceDistance);
                for(int i = 0; i < count; i++)
                {
                    var hit = hits[i];
                    if(hit.collider.gameObject == gameObject)
                        continue;
                    Vector2 obstaclePos = hit.collider.transform.position;
                    Vector2 deltaObstaclePos = obstaclePos - (Vector2)entityPosition;
                    Vector2 obstacleDir =  deltaObstaclePos.normalized;
                    Vector2 avoidanceDir = (Vector2)dir - obstacleDir;
                    avoidanceVelocity = entitySpeed * avoidanceDir.normalized * (1.0f-(deltaObstaclePos.magnitude/maxAvoidanceDistance));
                }
            }
            var avoidanceForce = body.mass * avoidanceVelocity / Time.fixedDeltaTime;
            if (avoidanceForce.magnitude > maxSteeringForce)
            {
                avoidanceForce = avoidanceForce.normalized * maxSteeringForce;
            }
            force += avoidanceForce;
            body.AddForce(force);
        }
    }
}

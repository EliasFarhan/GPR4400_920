using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FlockingManager : MonoBehaviour
{
    private Boid[] boids;
    private Vector3[] boidVelocities;
    [SerializeField] private Boid boidPrefab;
    [SerializeField] private int boidNumber = 100;
    [SerializeField] private float radius = 2.0f;
    [SerializeField] private float separationRadius = 1.0f;

    [SerializeField] private float separationWeight = 1.5f;
    [SerializeField] private float alignWeight = 1.0f;
    [SerializeField] private float cohesionWeight = 1.0f;
    [SerializeField] private float centerForceWeight = 3.0f;

    [SerializeField] private float maxSpeed = 5.0f;
    [SerializeField] private float radiusFromCenter = 10.0f;
    
    private void Start()
    { 
        var mainCamera = Camera.main;
        var cameraSize = 2.0f * mainCamera.orthographicSize * new Vector2(mainCamera.aspect, 1.0f);
        var cameraRect = new Rect(){min=-cameraSize/2.0f, max = cameraSize/2.0f};
        boids = new Boid[boidNumber];
        boidVelocities = new Vector3[boidNumber];
        for (int i = 0; i < boidNumber; i++)
        {
            var spawnPosition = new Vector3(Random.Range(cameraRect.xMin, cameraRect.xMax),
                Random.Range(cameraRect.yMin, cameraRect.yMax));
            var velocity = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
            var boid = Instantiate(boidPrefab, spawnPosition, Quaternion.identity, transform);
            boid.Velocity = velocity;
            boids[i] = boid;
        }
    }

    private void Update()
    {
        for(int i = 0;i < boidNumber; i++)
        {
            var currentBoid = boids[i];
            Vector3 localHeading = Vector3.zero;
            Vector3 localPositions = Vector3.zero;
            Vector3 localCloseBoids = Vector3.zero;
            
            int localBoidCount = 0;
            int separationCount = 0;
            foreach (var otherBoid in boids)
            {
                if(currentBoid == otherBoid)
                    continue;
                if((currentBoid.Position-otherBoid.Position).sqrMagnitude > radius*radius)
                    continue;
                localBoidCount++;
                localHeading += otherBoid.Velocity.normalized;
                localPositions += otherBoid.Position;
                if((currentBoid.Position-otherBoid.Position).sqrMagnitude > separationRadius*separationRadius)
                    continue;
                localCloseBoids += (currentBoid.Position-otherBoid.Position).normalized / (currentBoid.Position-otherBoid.Position).magnitude;
                separationCount++;
            }
            var currentVelocity = currentBoid.Velocity;
            if (separationCount > 0)
            {
                //Separation
                localCloseBoids /= separationCount;
                currentVelocity += localCloseBoids * (Time.deltaTime * separationWeight);
            }
            if (localBoidCount > 0)
            {
                
                //Alignement
                localHeading /= localBoidCount;
                currentVelocity += (localHeading.normalized - currentBoid.Velocity.normalized).normalized * (Time.deltaTime * alignWeight);
                //Cohesion
                localPositions /= localBoidCount;
                currentVelocity += (localPositions-currentBoid.Position).normalized * (Time.deltaTime * cohesionWeight);
            }
            
            //Steering to world center
            currentVelocity += currentBoid.Position.magnitude > radiusFromCenter
                ? -currentBoid.Position / currentBoid.Position.magnitude * (Time.deltaTime * centerForceWeight)
                : Vector3.zero;

            if (currentVelocity.sqrMagnitude > maxSpeed*maxSpeed)
            {
                currentVelocity = currentVelocity.normalized * maxSpeed;
            }
            boidVelocities[i] = currentVelocity;
        }

        for (int i = 0; i < boidNumber; i++)
        {
            boids[i].Velocity = boidVelocities[i];
        }
    }
}

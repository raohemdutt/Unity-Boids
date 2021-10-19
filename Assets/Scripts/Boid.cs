using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour {
    public static List<Boid> population;
    public BoidSettings boidSettings;
    private List<Boid> neighbors;
    private Vector3 boundaryCenter;
    private float boundaryRadius = 10;
    private Quaternion targetRotation;
    private Vector3 velocity, acceleration, separationForce, alignmentForce, cohesionForce;

    private void Awake() {
        if (population == null)
            population = new List<Boid>();
        population.Add(this);
        Initialize();
    }

    public void Initialize() {
        transform.forward = Random.insideUnitSphere.normalized;
        velocity = transform.forward * boidSettings.speed;
        acceleration = new Vector3(0,0,0);
    }

    public void SetBoundarySphere(Vector3 center, float radius) {
        boundaryCenter = center;
        boundaryRadius = radius;
    }

    private void FixedUpdate() {
        Flocking(); // handles Separation, Alignment, Cohesion forces
        Move();
        TurnAtBounds(); // makes sure boids turn back when they reach bounds
        ResetForces(); // set all forces to 0 since intertial bodies continue at same speed unless a force acts on them
    }

    private void ApplyForce(Vector3 force) {
        force /= boidSettings.mass;
        acceleration += force;
    }

    private void ResetForces() {
        acceleration = separationForce = alignmentForce = cohesionForce = Vector3.zero;
    }

    private void Move() {
        velocity = Vector3.zero;    // reset velocity
        if (boidSettings.moveFwd)
            velocity = transform.forward * boidSettings.speed;  // add forward movement
        velocity += acceleration * boidSettings.speed;  // add acceleration
        // Debug.Log("Velocity = " + velocity);
        transform.position += velocity * Time.fixedDeltaTime;   // move position
    }

    private void TurnAtBounds() {
        if (!boidSettings.boundsOn) {
            return;
        }
        else if ((transform.position - boundaryCenter).sqrMagnitude > (boundaryRadius * boundaryRadius) * 0.9f) {
            // targetRotation = Quaternion.Inverse(transform.rotation);
            targetRotation = Quaternion.LookRotation(boundaryCenter - transform.position + Random.onUnitSphere*boundaryRadius*0.5f);
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * boidSettings.speed);
    }

    private void Flocking() {
        FindNeighbors();
        Separation();
        Alignment();
        Cohesion();
    }

    private void FindNeighbors() {
        if (neighbors == null)
            neighbors = new List<Boid>();
        else
            neighbors.Clear();

        // sqrMagnitude is a bit faster than Magnitude since it doesn't require sqrt
        float sqrPerceptionRange = boidSettings.perceptionRange * boidSettings.perceptionRange;
        float sqrMagnitudeTemp = 0f;
        foreach (Boid other in population) {
            sqrMagnitudeTemp = (other.transform.position - transform.position).sqrMagnitude;
            if (sqrMagnitudeTemp < sqrPerceptionRange) {
                if (other != this) // skip self
                    neighbors.Add(other);
            }
        }
    }

    // Separation = Steer to avoid crowding local flockmates
    private void Separation() {
        if (boidSettings.separationStrength > 0) {
            if (neighbors == null || neighbors.Count <= 0) {
                return;
            } 
            else {
                foreach (Boid other in neighbors) {
                    // get the vector between self/other and flip it (avoid instead of seek)
                    separationForce -= other.transform.position-transform.position;
                }
                separationForce /= neighbors.Count; // get avg
                separationForce *= boidSettings.separationStrength;
                ApplyForce(separationForce);
                // Debug.Log("Separation Force " + separationForce);   
            }
        }
    }

    // Alignment = Steer towards the average heading of local flockmates
    private void Alignment() {
        if (boidSettings.alignmentStrength > 0) {
            if (neighbors == null || neighbors.Count <= 0) {
                return;
            }
            else {
                foreach (Boid other in neighbors) {

                }
            }
        }
    }

    // Cohesion = Steer to move toward the average position of local flockmates
    private void Cohesion() {
        if (boidSettings.cohesionStrength > 0) {
            if (neighbors == null || neighbors.Count <= 0) {
                return;
            }
            else {
                
            }
        }
    }


}

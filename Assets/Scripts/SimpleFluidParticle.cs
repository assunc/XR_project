using UnityEngine;
using System.Collections.Generic;

public class SimpleFluidParticle : MonoBehaviour
{

    public float moveSpeed = 1f;
    public float gravity = 9.81f;
    public float radius = 0.5f; // Particle interaction radius
    public float restDensity = 1000f; // Target density
    public float pressureMultiplier = 10f; // How strongly particles push away
    public LayerMask particleLayer; // Set this to your fluid particle layer

    private Vector3 velocity;
    private Vector3 predictedPosition;
    private float density;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        velocity = new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f)) * moveSpeed;
        
    }

    // Update is called once per frame
    void Update()
    {
        // Apply gravity
        velocity += Vector3.down * gravity * Time.deltaTime;
        
        // Predict next position
        predictedPosition = transform.position + velocity * Time.deltaTime;
        
        // Calculate density at predicted position
        CalculateDensity();
        
        // Apply density constraint (pressure)
        ApplyPressure();
        
        // Move particle
        transform.position += velocity * Time.deltaTime;
    }

    void CalculateDensity()
    {
        density = 0f;
        
        // Find nearby particles
        Collider[] nearbyParticles = Physics.OverlapSphere(predictedPosition, radius * 2f, particleLayer);
        
        foreach (Collider other in nearbyParticles)
        {
            if (other.gameObject != gameObject)
            {
                // Calculate distance
                float distance = Vector3.Distance(predictedPosition, other.transform.position);
                
                if (distance < radius * 2f)
                {
                    // Simple density calculation using smoothing kernel
                    float influence = Mathf.Max(0f, 1f - (distance / (radius * 2f)));
                    influence = influence * influence * influence; // Cubic falloff
                    
                    // Add to density
                    density += influence;
                }
            }
        }
    }

    void ApplyPressure()
    {
        // Only apply pressure if density is too high
        if (density > restDensity)
        {
            // Calculate pressure force
            float pressureForce = (density - restDensity) * pressureMultiplier;
            
            // Find nearby particles again
            Collider[] nearbyParticles = Physics.OverlapSphere(predictedPosition, radius * 2f, particleLayer);
            
            Vector3 pressureDirection = Vector3.zero;
            
            foreach (Collider other in nearbyParticles)
            {
                if (other.gameObject != gameObject)
                {
                    // Direction away from other particle
                    Vector3 direction = predictedPosition - other.transform.position;
                    float distance = direction.magnitude;
                    
                    if (distance > 0.001f && distance < radius * 2f)
                    {
                        // Normalize and scale by influence
                        float influence = Mathf.Max(0f, 1f - (distance / (radius * 2f)));
                        influence = influence * influence; // Quadratic falloff
                        
                        // Add to pressure direction
                        pressureDirection += direction.normalized * influence;
                    }
                }
            }
            
            // Apply pressure force to velocity
            if (pressureDirection.magnitude > 0.001f)
            {
                velocity += pressureDirection.normalized * pressureForce * Time.deltaTime;
            }
        }
    }

}

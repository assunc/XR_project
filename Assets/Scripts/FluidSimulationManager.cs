using UnityEngine;

public class FluidSimulationManager : MonoBehaviour
{
    public GameObject particlePrefab;
    public int numParticles = 20;
    public float spawnRadius = 5f;
    public float moveSpeed = 1f;
    public Transform enclosure;
    public float gravity = 9.8f;  // Added gravity
    
    // Positioning options
    public enum SpawnPattern { Random, Cube, Sphere, Waterfall }
    public SpawnPattern spawnPattern = SpawnPattern.Random;
    public Vector3 spawnPosition = Vector3.zero;
    public Vector3 spawnDimensions = new Vector3(2f, 2f, 2f);
    
    private GameObject[] particles;
    private Vector3[] positions;
    private Vector3[] velocities;
    private Bounds enclosureBounds;
    
    void Start()
    {
        if (particlePrefab == null || enclosure == null)
        {
            Debug.LogError("Particle Prefab or Enclosure not assigned!");
            enabled = false;
            return;
        }
        
        Collider enclosureCollider = enclosure.GetComponent<Collider>();
        if (enclosureCollider == null)
        {
            Debug.LogError("Enclosure has no Collider!");
            enabled = false;
            return;
        }
        
        enclosureBounds = enclosureCollider.bounds;
        particles = new GameObject[numParticles];
        positions = new Vector3[numParticles];
        velocities = new Vector3[numParticles];
        
        InitializeParticles();
    }
    
    void InitializeParticles()
    {
        switch (spawnPattern)
        {
            case SpawnPattern.Random:
                SpawnRandomPattern();
                break;
            case SpawnPattern.Cube:
                SpawnCubePattern();
                break;
            case SpawnPattern.Sphere:
                SpawnSpherePattern();
                break;
            case SpawnPattern.Waterfall:
                SpawnWaterfallPattern();
                break;
        }
    }
    
    void SpawnRandomPattern()
    {
        for (int i = 0; i < numParticles; i++)
        {
            positions[i] = enclosureBounds.center + Random.insideUnitSphere * spawnRadius;
            // Give initial velocity
            velocities[i] = Random.insideUnitSphere * moveSpeed;
            particles[i] = Instantiate(particlePrefab, positions[i], Quaternion.identity);
        }
    }
    
    void SpawnCubePattern()
    {
        int particlesPerAxis = Mathf.CeilToInt(Mathf.Pow(numParticles, 1f/3f));
        float spacing = Mathf.Min(spawnDimensions.x, spawnDimensions.y, spawnDimensions.z) / particlesPerAxis;
        
        int index = 0;
        for (int x = 0; x < particlesPerAxis && index < numParticles; x++)
        {
            for (int y = 0; y < particlesPerAxis && index < numParticles; y++)
            {
                for (int z = 0; z < particlesPerAxis && index < numParticles; z++)
                {
                    Vector3 offset = new Vector3(
                        x * spacing - (spawnDimensions.x / 2) + (spacing / 2),
                        y * spacing - (spawnDimensions.y / 2) + (spacing / 2),
                        z * spacing - (spawnDimensions.z / 2) + (spacing / 2)
                    );
                    
                    positions[index] = spawnPosition + offset;
                    // Small initial velocity
                    velocities[index] = new Vector3(
                        Random.Range(-0.1f, 0.1f),
                        Random.Range(-0.1f, 0.1f),
                        Random.Range(-0.1f, 0.1f)
                    ) * moveSpeed;
                    
                    particles[index] = Instantiate(particlePrefab, positions[index], Quaternion.identity);
                    index++;
                }
            }
        }
    }
    
    void SpawnSpherePattern()
    {
        float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
        float angleIncrement = Mathf.PI * 2 * goldenRatio;
        
        for (int i = 0; i < numParticles; i++)
        {
            float t = (float)i / numParticles;
            float inclination = Mathf.Acos(1 - 2 * t);
            float azimuth = angleIncrement * i;
            
            float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
            float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
            float z = Mathf.Cos(inclination);
            
            Vector3 pos = new Vector3(x, y, z) * spawnRadius;
            positions[i] = spawnPosition + pos;
            // Small inward velocity
            velocities[i] = -pos.normalized * moveSpeed * 0.2f;
            particles[i] = Instantiate(particlePrefab, positions[i], Quaternion.identity);
        }
    }
    
    void SpawnWaterfallPattern()
    {
        Vector3 topPosition = new Vector3(
            spawnPosition.x,
            enclosureBounds.max.y - 0.5f,
            spawnPosition.z
        );
        
        for (int i = 0; i < numParticles; i++)
        {
            Vector3 pos = topPosition + new Vector3(
                Random.Range(-spawnDimensions.x/2, spawnDimensions.x/2),
                0,
                Random.Range(-spawnDimensions.z/2, spawnDimensions.z/2)
            );
            
            positions[i] = pos;
            // Small initial downward velocity
            velocities[i] = new Vector3(0, -0.1f, 0) * moveSpeed;
            particles[i] = Instantiate(particlePrefab, positions[i], Quaternion.identity);
        }
    }
    
    void Update()
    {
        for (int i = 0; i < numParticles; i++)
        {
            // Apply gravity
            velocities[i] += Vector3.down * gravity * Time.deltaTime;
            
            // Update position
            positions[i] += velocities[i] * Time.deltaTime;
            
            // Boundary collision with energy loss
            float bounceFactor = 0.7f; // Energy loss on bounce
            
            if (positions[i].x < enclosureBounds.min.x)
            {
                positions[i].x = enclosureBounds.min.x;
                velocities[i].x = -velocities[i].x * bounceFactor;
            }
            else if (positions[i].x > enclosureBounds.max.x)
            {
                positions[i].x = enclosureBounds.max.x;
                velocities[i].x = -velocities[i].x * bounceFactor;
            }
            
            if (positions[i].y < enclosureBounds.min.y)
            {
                positions[i].y = enclosureBounds.min.y;
                velocities[i].y = -velocities[i].y * bounceFactor;
            }
            else if (positions[i].y > enclosureBounds.max.y)
            {
                positions[i].y = enclosureBounds.max.y;
                velocities[i].y = -velocities[i].y * bounceFactor;
            }
            
            if (positions[i].z < enclosureBounds.min.z)
            {
                positions[i].z = enclosureBounds.min.z;
                velocities[i].z = -velocities[i].z * bounceFactor;
            }
            else if (positions[i].z > enclosureBounds.max.z)
            {
                positions[i].z = enclosureBounds.max.z;
                velocities[i].z = -velocities[i].z * bounceFactor;
            }
            
            // Update particle position
            particles[i].transform.position = positions[i];
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        
        switch (spawnPattern)
        {
            case SpawnPattern.Cube:
                Gizmos.DrawWireCube(spawnPosition, spawnDimensions);
                break;
            case SpawnPattern.Sphere:
                Gizmos.DrawWireSphere(spawnPosition, spawnRadius);
                break;
            case SpawnPattern.Waterfall:
                Vector3 topPosition = new Vector3(
                    spawnPosition.x,
                    enclosure ? enclosure.GetComponent<Collider>().bounds.max.y - 0.5f : spawnPosition.y,
                    spawnPosition.z
                );
                Gizmos.DrawWireCube(topPosition, new Vector3(spawnDimensions.x, 0.1f, spawnDimensions.z));
                break;
        }
    }

}
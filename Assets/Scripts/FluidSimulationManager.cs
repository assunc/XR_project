using UnityEngine;

public class FluidSimulationManager : MonoBehaviour
{
    public GameObject particlePrefab;
    public int numParticles = 20;
    public float spawnRadius = 5f;
    public float moveSpeed = 1f;
    public Transform enclosure; // Assign your enclosure GameObject here

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

        for (int i = 0; i < numParticles; i++)
        {
            positions[i] = enclosureBounds.center + Random.insideUnitSphere * spawnRadius;
            velocities[i] = Random.insideUnitSphere * moveSpeed;
            particles[i] = Instantiate(particlePrefab, positions[i], Quaternion.identity);
        }
    }

    void Update()
    {
        for (int i = 0; i < numParticles; i++)
        {
            positions[i] += velocities[i] * Time.deltaTime;

            // Simple boundary collision
            if (positions[i].x < enclosureBounds.min.x || positions[i].x > enclosureBounds.max.x)
            {
                velocities[i].x *= -1;
            }
            if (positions[i].y < enclosureBounds.min.y || positions[i].y > enclosureBounds.max.y)
            {
                velocities[i].y *= -1;
            }
            if (positions[i].z < enclosureBounds.min.z || positions[i].z > enclosureBounds.max.z)
            {
                velocities[i].z *= -1;
            }

            particles[i].transform.position = positions[i];
        }
    }
}
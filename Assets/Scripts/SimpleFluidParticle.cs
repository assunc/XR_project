using UnityEngine;

public class SimpleFluidParticle : MonoBehaviour
{

    public float moveSpeed = 1f;
    public float gravity = 9.81f;
    private Vector3 velocity;
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

        transform.position += velocity * Time.deltaTime;
        
    }
}

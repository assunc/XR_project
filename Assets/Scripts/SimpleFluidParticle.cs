using UnityEngine;

public class SimpleFluidParticle : MonoBehaviour
{

    public float moveSpeed = 1f;
    private Vector3 velocity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        velocity = Random.insideUnitSphere * moveSpeed;
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += velocity * Time.deltaTime;
        
    }
}

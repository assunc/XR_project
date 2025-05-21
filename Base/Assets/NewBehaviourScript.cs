using Unity.Mathematics;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    int t = 0;
    // Update is called once per frame
    void Update()
    {
        transform.position = new float3(transform.position.x, transform.position.y, transform.position.z  + (float)0.05*math.sin(t++*4*math.PI/180));
    }
}

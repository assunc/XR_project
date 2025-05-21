using UnityEngine;

public class TestPokeResponder : MonoBehaviour
{
    private Renderer _renderer;
    private Color originalColor;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        originalColor = _renderer.material.color;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Visual Feedback
        _renderer.material.color = Color.green;

        // Optional Debug Message
        Debug.Log("Poke detected: " + collision.gameObject.name);
    }

    void OnCollisionExit(Collision collision)
    {
        _renderer.material.color = originalColor;
    }
}
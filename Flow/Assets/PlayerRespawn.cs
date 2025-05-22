using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public Transform player;             // The player object (e.g., XR Rig)
    public Transform respawnPoint;       // Where the player respawns
    public float fallThreshold = -5f;    // Y value below which player is considered "out of bounds"

    void Update()
    {
        if (player.position.y < fallThreshold)
        {
            Respawn();
        }
    }

    void Respawn()
    {
        CharacterController controller = player.GetComponent<CharacterController>();
        
        if (controller != null)
        {
            // Disable CharacterController temporarily to allow manual repositioning
            controller.enabled = false;
            player.position = respawnPoint.position;
            controller.enabled = true;
        }
        else
        {
            // If no CharacterController, just set position
            player.position = respawnPoint.position;
        }
    }
}
